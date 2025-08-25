using System.Data;
using Dapper;
using Core.Application.Common.Interfaces.Item.PutAway;
using Core.Application.Item.PutAway.Queries.GetAllPutAwayRule;
using Core.Application.Item.PutAway.Queries.GetPutAwayTargets;
using Contracts.Interfaces.External.IWarehouse; // IRackGrpcClient, IBinGrpcClient

namespace InventoryManagement.Infrastructure.Repositories.Item.Templates
{
    public sealed class PutAwayRuleQueryRepository : IPutAwayRuleQueryRepository
    {
        private readonly IDbConnection _db;                 
        private readonly IRackGrpcClient _rackClient;       
        private readonly IBinGrpcClient _binClient;    
         private readonly IWarehouseGrpcClient _whClient;

        public PutAwayRuleQueryRepository(
            IDbConnection db,
            IRackGrpcClient rackClient,
            IBinGrpcClient binClient, IWarehouseGrpcClient whClient)
        {
            _db = db;
            _rackClient = rackClient;
            _binClient = binClient;
            _whClient = whClient;
        }

        public async Task<(IEnumerable<PutAwayRuleListDto> rows, int total)> GetPagedAsync(
            int page, int size, string? search, CancellationToken ct = default)
        {
            var skip = (page - 1) * size;
            var sql = """
            ;WITH R AS (
              SELECT 
                r.Id,
                r.UnitId,
                r.ItemGroupId,
                ig.ItemGroupName     AS ItemGroupName,
                r.ItemCategoryId,
                ic.ItemCategoryName  AS ItemCategoryName,
                r.ItemId,
                im.ItemName          AS ItemName,
                r.WarehouseId
              FROM Inventory.PutAwayRule r
              JOIN Inventory.ItemGroup    ig ON ig.Id = r.ItemGroupId    AND ig.IsDeleted = 0
              JOIN Inventory.ItemCategory ic ON ic.Id = r.ItemCategoryId AND ic.IsDeleted = 0
              LEFT JOIN Inventory.ItemMaster im ON im.Id = r.ItemId       AND im.IsDeleted = 0
              WHERE r.IsDeleted = 0
                AND (
                    @Search IS NULL
                    OR ig.ItemGroupName    LIKE @Like
                    OR ic.ItemCategoryName LIKE @Like
                    OR im.ItemName         LIKE @Like
                )
            )
            SELECT * FROM R
            ORDER BY Id DESC
            OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY;    

            SELECT COUNT(*)
            FROM Inventory.PutAwayRule r
            JOIN Inventory.ItemGroup    ig ON ig.Id = r.ItemGroupId    AND ig.IsDeleted = 0
            JOIN Inventory.ItemCategory ic ON ic.Id = r.ItemCategoryId AND ic.IsDeleted = 0
            LEFT JOIN Inventory.ItemMaster im ON im.Id = r.ItemId       AND im.IsDeleted = 0
            WHERE r.IsDeleted = 0
              AND (
                  @Search IS NULL
                  OR ig.ItemGroupName    LIKE @Like
                  OR ic.ItemCategoryName LIKE @Like
                  OR im.ItemName         LIKE @Like
              );  
            """;

            var like = string.IsNullOrWhiteSpace(search) ? null : $"%{search}%";
            using var m = await _db.QueryMultipleAsync(
                new CommandDefinition(sql, new { skip, size, Search = search, Like = like }, cancellationToken: ct));

            var rows = (await m.ReadAsync<PutAwayRuleListDto>()).ToList();
            var total = await m.ReadSingleAsync<int>();
            
           // inject IWarehouseLookupGrpcClient as _whLookup (not _whClient)
            if (rows.Count > 0)
            {
                var whIds = rows.Select(r => r.WarehouseId).Distinct().ToArray();

                var tasks = whIds.Select(id => _whClient.GetByIdAsync(id, ct));
                var results = await Task.WhenAll(tasks); // WarehouseDto?[]

                var dict = results
                    .Where(w => w != null)
                    .ToDictionary(w => w!.Id, w => w!);

                foreach (var r in rows)
                {
                    if (dict.TryGetValue(r.WarehouseId, out var w))
                    {
                        r.WarehouseCode = w.WarehouseCode; // note property names on your DTO
                        r.WarehouseName = w.WarehouseName;
                    }
                }
            }
            return (rows, total);
        }

        public async Task<PutAwayRuleDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
           var sql = """
            SELECT 
                r.Id,
                r.UnitId,
                r.ItemGroupId,
                ig.ItemGroupName     AS ItemGroupName,    
                r.ItemCategoryId,
                ic.ItemCategoryName  AS ItemCategoryName, 
                r.ItemId,
                im.ItemName          AS ItemName,         
                r.WarehouseId
            FROM Inventory.PutAwayRule r
            JOIN Inventory.ItemGroup     ig ON ig.Id = r.ItemGroupId     AND ig.IsDeleted = 0
            JOIN Inventory.ItemCategory  ic ON ic.Id = r.ItemCategoryId  AND ic.IsDeleted = 0
            LEFT JOIN Inventory.ItemMaster im ON im.Id = r.ItemId        AND im.IsDeleted = 0            
            WHERE r.Id = @id AND r.IsDeleted = 0;

            SELECT s.Id, s.PutAwayRuleId, s.StorageTypeId, s.TargetId, s.PriorityId,MM.code PriorityName
            FROM Inventory.PutAwayStrategy s
            JOIN Inventory.MiscMaster     MM ON MM.Id = s.priorityId     AND MM.IsDeleted = 0
            WHERE s.PutAwayRuleId = @id AND s.IsDeleted = 0
            ORDER BY s.PriorityId ASC; 
            """;

            using var m = await _db.QueryMultipleAsync(new CommandDefinition(sql, new { id }, cancellationToken: ct));
            var head = await m.ReadSingleOrDefaultAsync<PutAwayRuleDetailDto>();
            if (head == null) return null;
            head.Strategies = (await m.ReadAsync<PutAwayStrategyDto>()).ToList();
            // Warehouse enrichment (single id)
            var wh = await _whClient.GetByIdAsync(head.WarehouseId, ct);
            if (wh != null)
            {
                head.WarehouseCode = wh.WarehouseCode;
                head.WarehouseName = wh.WarehouseName;
            }
            // ---- Enrich targets via gRPC (Rack/Bin) ----
            var missingTypeIds = head.Strategies
                .Where(s => string.IsNullOrWhiteSpace(s.StorageTypeCode))
                .Select(s => s.StorageTypeId)
                .Distinct()
                .ToArray();

            if (missingTypeIds.Length > 0)
            {
                const string typeSql = "SELECT Id, Code FROM Inventory.MiscMaster WHERE Id IN @Ids AND IsDeleted = 0;";
                var types = await _db.QueryAsync<(int Id, string? Code)>(
                    new CommandDefinition(typeSql, new { Ids = missingTypeIds }, cancellationToken: ct));
                var typeDict = types.Where(t => !string.IsNullOrWhiteSpace(t.Code))
                                    .ToDictionary(t => t.Id, t => t.Code!);

                foreach (var s in head.Strategies.Where(x => string.IsNullOrWhiteSpace(x.StorageTypeCode)))
                    if (typeDict.TryGetValue(s.StorageTypeId, out var code)) s.StorageTypeCode = code;
            }

            // ---- Enrich TargetCode/TargetName via gRPC based on storage type of EACH strategy ----
            string Norm(string? s) =>
                string.IsNullOrWhiteSpace(s) ? "" : s.Trim().ToUpperInvariant().Replace(" ", "").Replace("_", "");

            var binIds  = head.Strategies.Where(s => s.TargetId.HasValue && Norm(s.StorageTypeCode) == "BIN")
                                        .Select(s => s.TargetId!.Value).Distinct().ToArray();
            var rackIds = head.Strategies.Where(s => s.TargetId.HasValue && Norm(s.StorageTypeCode) == "RACK")
                                        .Select(s => s.TargetId!.Value).Distinct().ToArray();

            // parallel gRPC fetches
            var binTasks  = binIds.Select(id2  => _binClient.GetByIdAsync(id2, ct));
            var rackTasks = rackIds.Select(id2 => _rackClient.GetByIdAsync(id2, ct));
            var binResults  = await Task.WhenAll(binTasks);
            var rackResults = await Task.WhenAll(rackTasks);

            var binById  = binResults.Where(b => b != null).ToDictionary(b => b!.Id,  b => b!);
            var rackById = rackResults.Where(r => r != null).ToDictionary(r => r!.Id, r => r!);

            foreach (var s in head.Strategies)
            {
                if (!s.TargetId.HasValue) continue;

                var t = Norm(s.StorageTypeCode);
                if (t == "BIN" && binById.TryGetValue(s.TargetId.Value, out var b))
                {
                    s.TargetCode = b.BinCode;
                    s.TargetName = b.BinName;
                }
                else if (t == "RACK" && rackById.TryGetValue(s.TargetId.Value, out var r))
                {
                    s.TargetCode = r.RackCode;
                    s.TargetName = r.RackName;
                }
                // OpenSpace/others: add when those services exist
            }
            return head;
        }

      /*   // -----------------------------
        // Evaluate (now uses gRPC for Rack/Bin)
        // -----------------------------
        public async Task<PutAwayEvaluateResult?> EvaluateAsync(PutAwayEvaluateRequest req, CancellationToken ct = default)
        {
            // 1) Find the most specific matching rule
            const string ruleSql = """
            SELECT TOP 1 *
            FROM Inventory.PutAwayRule r
            WHERE r.IsDeleted = 0
              AND r.UnitId = @UnitId
              AND r.WarehouseId = @WarehouseId
              AND r.ItemGroupId = @ItemGroupId
              AND r.ItemCategoryId = @ItemCategoryId
              AND ((r.ItemId IS NULL) OR r.ItemId = @ItemId)
            ORDER BY CASE WHEN r.ItemId IS NULL THEN 1 ELSE 0 END, r.Id DESC;
            """;

            var rule = await _db.QuerySingleOrDefaultAsync<dynamic>(
                new CommandDefinition(ruleSql, req, cancellationToken: ct));
            if (rule is null) return null;

            // 2) Active strategies with their Misc code + priority sortOrder
            const string stratSql = """
            SELECT 
                s.Id,
                s.PutAwayRuleId,
                s.StorageTypeId,
                mm.Code AS StorageTypeCode,     -- Inventory.MiscMaster
                s.TargetId,
                s.PriorityId,
                COALESCE(mp.sortOrder, s.PriorityId) AS PriorityRank
            FROM Inventory.PutAwayStrategy s
            JOIN Inventory.MiscMaster mm ON mm.Id = s.StorageTypeId AND mm.IsActive = 1 AND mm.IsDeleted = 0
            LEFT JOIN Inventory.MiscMaster mp ON mp.Id = s.PriorityId AND mp.IsActive = 1 AND mp.IsDeleted = 0
            WHERE s.PutAwayRuleId = @RuleId AND s.IsDeleted = 0
            ORDER BY PriorityRank ASC, s.Id ASC;
            """;

            var strategies = (await _db.QueryAsync<dynamic>(
                new CommandDefinition(stratSql, new { RuleId = (int)rule.Id }, cancellationToken: ct)))
                .ToList();

            // 3) Evaluate each strategy in priority order
            foreach (var s in strategies)
            {
                var code = Normalize((string)s.StorageTypeCode);
                var whId = (int)rule.WarehouseId;

                if (code == "BIN")
                {
                    // Dynamic bin in the warehouse (no specific rack)
                    var bins = await _binClient.GetAllBinMasterAsync(whId, rackId: null, search: null, onlyActive: true, ct: ct);
                    var cand = PickFirstUsableBin(bins, req);
                    if (cand is not null)
                        return new PutAwayEvaluateResult
                        {
                            RuleId        = (int)rule.Id,
                            StrategyId    = (int)s.Id,
                            StorageTypeId = (int)s.StorageTypeId,
                            TargetId      = cand.Id,
                            LocationCode  = cand.BinCode
                        };
                }
                else if (code == "RACK")
                {
                    // Rack strategy: TargetId is RackId. Find a bin under that rack.
                    var rackId = (int?)s.TargetId ?? 0;
                    if (rackId > 0)
                    {
                        var bins = await _binClient.GetAllBinMasterAsync(whId, rackId: rackId, search: null, onlyActive: true, ct: ct);
                        var cand = PickFirstUsableBin(bins, req);
                        if (cand is not null)
                            return new PutAwayEvaluateResult
                            {
                                RuleId        = (int)rule.Id,
                                StrategyId    = (int)s.Id,
                                StorageTypeId = (int)s.StorageTypeId,
                                TargetId      = cand.Id,
                                LocationCode  = cand.BinCode
                            };
                    }
                }
                else if (code == "OPENSPACE")
                {
                    // TODO: If OpenSpace is exposed via gRPC, call that client here.
                    // Otherwise, implement local lookup similar to your prior SQL.
                }
            }

            return null; // nothing matched
        } 

        // -----------------------------
        // Lookup targets (now uses gRPC for Rack/Bin)
        // -----------------------------
        public async Task<List<PutAwayTargetLookupDto>> GetTargetsByMiscAsync(
            int warehouseId,
            int storageTypeMiscId,
            string? searchPattern,
            CancellationToken ct = default)
        {
            // Read live code from Inventory.MiscMaster
            const string miscSql = """
            SELECT Code
            FROM Inventory.MiscMaster
            WHERE Id = @Id AND IsActive = 1 AND IsDeleted = 0;
            """;

            var raw = await _db.ExecuteScalarAsync<string?>(
                new CommandDefinition(miscSql, new { Id = storageTypeMiscId }, cancellationToken: ct));

            if (string.IsNullOrWhiteSpace(raw)) return new();

            var code = Normalize(raw);

            if (code == "RACK")
            {
                // Your Rack client returns paged data → pull a reasonable page and filter by WarehouseId
                var page = await _rackClient.GetAllAsync(pageNumber: 1, pageSize: 200, search: searchPattern, ct);
                return page.Items
                           .Where(r => r.WarehouseId == warehouseId)
                           .Select(r => new PutAwayTargetLookupDto { Id = r.Id, Code = r.RackCode, Name = r.RackName })
                           .ToList();
            }

            if (code == "BIN")
            {
                var bins = await _binClient.GetAllBinMasterAsync(warehouseId, rackId: null, search: searchPattern, onlyActive: true, ct: ct);
                return bins.Select(b => new PutAwayTargetLookupDto { Id = b.Id, Code = b.BinCode, Name = b.BinName })
                           .ToList();
            }

            if (code == "OPENSPACE")
            {
                // TODO: call OpenSpace gRPC once available; return [] for now
                return new();
            }

            return new();
        }*/

        private static string Normalize(string text) =>
            string.IsNullOrWhiteSpace(text) ? string.Empty : text.Trim().ToUpperInvariant().Replace(" ", "").Replace("_", "");

        private static Contracts.Dtos.Warehouse.BinDto? PickFirstUsableBin(
            List<Contracts.Dtos.Warehouse.BinDto> bins,
            PutAwayEvaluateRequest req)
        {            
            return bins.FirstOrDefault();
        }
    }
}
