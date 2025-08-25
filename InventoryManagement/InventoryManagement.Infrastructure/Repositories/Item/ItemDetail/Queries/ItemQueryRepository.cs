using System.Data;
using Contracts.Interfaces.External.IParty;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Application.Item.ItemDetail.Queries.GetItemAutoComplete;
using Core.Domain.Common;
using Core.Domain.Entities.Item.ItemDetail.Variant;
using Dapper;
using Grpc.Core;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Queries
{
    public sealed class ItemQueryRepository : IItemQueryRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IDbConnection _dbConnection;
        private readonly IUnitGrpcClient _unitGrpcClient;
        private readonly IPartyGrpcClient _partyGrpcClient;
        private readonly ICountryGrpcClient _countryGrpcClient;

        public ItemQueryRepository(IDbConnection dbConnection, ApplicationDbContext db, IUnitGrpcClient unitGrpcClient, IPartyGrpcClient partyGrpcClient, ICountryGrpcClient countryGrpcClient)
        {
            _db = db;
            _dbConnection = dbConnection;
            _unitGrpcClient = unitGrpcClient;
            _partyGrpcClient = partyGrpcClient;
            _countryGrpcClient = countryGrpcClient;
        }

        public async Task<(List<ItemListDto> Items, int TotalCount)> GetAllAsync(int page, int size, string? search, bool onlyActive,int? itemGroupId,int? itemCategoryId, CancellationToken ct = default)
        {
            var q = _db.ItemMaster.AsNoTracking()
                .Where(x => x.IsDeleted == BaseEntity.IsDelete.NotDeleted);

            if (onlyActive) q = q.Where(x => x.IsActive == BaseEntity.Status.Active);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                q = q.Where(x => x.ItemCode.Contains(term) || x.ItemName.Contains(term));
            }
            if (itemGroupId.HasValue && itemGroupId.Value > 0)
                 q = q.Where(x => x.ItemGroupId == itemGroupId.Value);

            if (itemCategoryId.HasValue && itemCategoryId.Value > 0)
                q = q.Where(x => x.ItemCategoryId == itemCategoryId.Value);
            var total = await q.CountAsync(ct);
            var list = await q.OrderBy(x => x.ItemName)
                .Skip((page - 1) * size).Take(size)
                .Select(x => new ItemListDto
                {
                    Id = x.Id,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    HasVariants = x.HasVariants,
                    IsStockItem = x.IsStockItem,
                    UnitId = x.UnitId,
                    ParentItemName = x.ParentItem != null ? x.ParentItem.ItemName : null,
                    ItemGroupName   = x.ItemGroup != null ? x.ItemGroup.ItemGroupName : null,
                    ItemCategoryName= x.ItemCategory != null ? x.ItemCategory.ItemCategoryName : null
                })
                .ToListAsync(ct);

            return (list, total);
        }

        public async Task<ItemDetailsDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            // 1) Load the item (single EF query)
            var dto = await _db.ItemMaster
                .AsNoTracking()
                .Where(i => i.Id == id && i.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .Select(i => new ItemDetailsDto
                {
                    // base
                    Id = i.Id,
                    UnitId = i.UnitId,
                    ItemCode = i.ItemCode,
                    ItemName = i.ItemName,
                    HSNId = i.HSNId,
                    HSNCode = i.HSNMaster != null ? i.HSNMaster.HSNCode : null,
                    ItemGroupId = i.ItemGroupId,
                    ItemGroupName = i.ItemGroup != null ? i.ItemGroup.ItemGroupName : null,
                    ItemCategoryId = i.ItemCategoryId,
                    ItemCategoryName = i.ItemCategory != null ? i.ItemCategory.ItemCategoryName : null,
                    StockUomId = i.StockUomId,
                    StockUOM = i.UOM != null ? i.UOM.UOMName : null,
                    ItemClassificationId = i.ItemClassificationId,
                    ItemClassification = i.MiscClassification != null ? i.MiscClassification.Code : null,
                    Description = i.Description,
                    ValidFrom = i.ValidFrom,
                    XPlantMaterialStatusId = i.XPlantMaterialStatusId,
                    XPlantMaterialStatus = i.MiscStatus != null ? i.MiscStatus.Code : null,
                    IsStockItem = i.IsStockItem,
                    MaintainStock = i.MaintainStock,
                    HasVariants = i.HasVariants,
                    ParentItemId = i.ParentItemId,
                    ParentItemName = i.ParentItem != null ? i.ParentItem.ItemName : null,
                    ItemImage = i.ItemImage,

                    // tabs (1-1)
                    Purchase = i.Purchase == null ? null : new ItemPurchaseDetailDto
                    {
                        PurchaseUomId = i.Purchase.PurchaseUomId,
                        PurchaseUOM = i.Purchase.PurchaseUOM != null ? i.Purchase.PurchaseUOM.UOMName : null,
                        LeadTimeDays = i.Purchase.LeadTimeDays,
                        SafetyStock = i.Purchase.SafetyStock,
                        GrProcessingTimeDays = i.Purchase.GrProcessingTimeDays,
                        AutomaticPo = i.Purchase.AutomaticPo,
                        OriginCountryId = i.Purchase.OriginCountryId,
                        TariffNumber = i.Purchase.TariffNumber
                    },
                    Inventory = i.Inventory == null ? null : new ItemInventoryDetailDto
                    {
                        InventoryUOM = i.Inventory.WeightUOM != null ? i.Inventory.WeightUOM.UOMName : null,
                        DefaultMaterialRequestType = i.MiscStatus != null ? i.MiscStatus.Code : null,
                        ValuationMethod = i.MiscStatus != null ? i.MiscStatus.Code : null,
                        RequestType = i.MiscStatus != null ? i.MiscStatus.Code : null,
                        Weight = i.Inventory.Weight,
                        WeightUomId = i.Inventory.WeightUomId,
                        DefaultMaterialRequestTypeId = i.Inventory.DefaultMaterialRequestTypeId,
                        ValuationMethodId = i.Inventory.ValuationMethodId,
                        ShelfLife = i.Inventory.ShelfLife,
                        UpperTolerance = i.Inventory.UpperTolerance,
                        LowerTolerance = i.Inventory.LowerTolerance,
                        BatchNumberSeries = i.Inventory.BatchNumberSeries,
                        SerialNumberSeries = i.Inventory.SerialNumberSeries,
                        ReorderLevel = i.Inventory.ReorderLevel,
                        ReorderQty = i.Inventory.ReorderQty,
                        RequestTypeId = i.Inventory.RequestTypeId,
                        AllowNegativeStock = i.Inventory.AllowNegativeStock,
                        BatchManagement = i.Inventory.BatchManagement,
                        ApplyBatchNumber = i.Inventory.ApplyBatchNumber
                    },
                    Quality = i.Quality == null ? null : new ItemQualityDetailDto
                    {
                        InspectionTemplateId = i.Quality.InspectionTemplateId,
                        CertificateTypeId = i.Quality.CertificateTypeId,
                        InspLotProcessingTime = i.Quality.InspLotProcessingTime,
                        InspectionRequired = i.Quality.InspectionRequired,
                        QualityInspectionFree = i.Quality.QualityInspectionFree,
                        IsCertificateRequiredFromSupplier = i.Quality.IsCertificateRequiredFromSupplier,
                        CertificateType = i.MiscStatus != null ? i.MiscStatus.Code : null,
                    },

                    // collections
                    Suppliers = i.Suppliers
                        .OrderBy(s => s.SupplierId)
                        .Select(s => new ItemSupplierDetailDto
                        {
                            SupplierId = s.SupplierId,
                            UnitId = s.UnitId,
                            SupplierPartNo = s.SupplierPartNo,
                            MOQ = s.MOQ,
                            MOQUomId = s.MOQUomId,
                            PackageValue = s.PackageValue,
                            PackageUomId = s.PackageUomId,
                            DefaultSupplier = s.DefaultSupplier,
                            LeadTime = s.LeadTime
                        })
                        .ToList(),

                    Manufacture = i.Manufacture
                        .OrderBy(m => m.UnitId)
                        .Select(m => new ItemManufactureDetailDto
                        {
                            UnitId = m.UnitId,
                            ManufacturingTypeId = m.ManufacturingTypeId,
                            ManufacturingType = _db.MiscMaster
                                .Where(mm => mm.Id == m.ManufacturingTypeId)
                                .Select(mm => mm.Code)
                                .FirstOrDefault()
                        })
                        .ToList(),

                    Uoms = i.ItemUOMs
                        .OrderBy(u => u.Id)
                        .Select(u => new ItemDetailUomDto
                        {
                            ConversionUOMId = u.ConversionUOMId,
                            ConversionRate = u.ConversionRate,
                            ConversionUOM = u.ConversionUOM != null ? u.ConversionUOM.UOMName : null
                        })
                        .ToList(),

                    VariantValues = _db.Set<ItemVariantValue>()
                        .Where(v => v.ItemId == i.Id)
                        .OrderBy(v => v.AttributeId)
                        .Select(v => new VariantDetailDto
                        {
                            AttributeId = v.AttributeId,
                            OptionValue = v.OptionValue,
                            VariantBasedOn = v.VariantBasedOn,
                            AttributeGroupId = v.AttributeGroupId,
                            VariantBasedOnValue = v.MiscVariantBasedOn != null ? v.MiscVariantBasedOn.Code : null,
                            AttributeGroup = v.MiscAttributeGroup != null ? v.MiscAttributeGroup.Description : null,
                            AttributeName = v.MiscAttribute != null ? v.MiscAttribute.Code : null
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync(ct);

            if (dto is null) return null;

            // 2) Enrich with Units (gRPC)
            var unitIds = new HashSet<int> { dto.UnitId };
            if (dto.Suppliers != null) foreach (var s in dto.Suppliers) unitIds.Add(s.UnitId);
            if (dto.Manufacture != null) foreach (var m in dto.Manufacture) unitIds.Add(m.UnitId);

            var units = await _unitGrpcClient.GetAllUnitAsync();
            var unitMap = units.ToDictionary(u => u.UnitId, u => u.UnitName);

            dto.UnitName = unitMap.TryGetValue(dto.UnitId, out var uName) ? uName : null;
            if (dto.Suppliers != null)
                foreach (var s in dto.Suppliers)
                    s.UnitName = unitMap.TryGetValue(s.UnitId, out var n) ? n : null;
            if (dto.Manufacture != null)
                foreach (var m in dto.Manufacture)
                    m.UnitName = unitMap.TryGetValue(m.UnitId, out var n) ? n : null;

            // 3) Enrich with Countries (gRPC) — avoid .Result
            var countries = await _countryGrpcClient.GetAllCountryAsync();
            var countryMap = countries.ToDictionary(x => x.CountryId, x => x.CountryName);
            if (dto.Purchase?.OriginCountryId is int cid)
                dto.Purchase.CountryName = countryMap.TryGetValue(cid, out var cn) ? cn : null;

           // inside ItemQueryRepository.GetByIdAsync (AFTER EF has finished and dto is materialized)
            if (dto.Suppliers != null && dto.Suppliers.Count > 0)
            {
                var supplierIds = dto.Suppliers
                    .Select(s => s.SupplierId)
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

                if (supplierIds.Count > 0)
                {
                    // Option A: sequential (simplest, avoids hammering)
                    var nameMap = new Dictionary<int, string>();
                    foreach (var sid in supplierIds)
                    {
                        try
                        {
                            var list = await _partyGrpcClient.GetAutoCompleteAsync(sid.ToString(), ct);
                            var exact = list.FirstOrDefault(p => p.Id == sid);

                            if (exact != null) nameMap[sid] = exact.PartyName;
                            else if (list.Count > 0) nameMap[sid] = list[0].PartyName;
                        }
                        catch (Grpc.Core.RpcException)
                        {
                            // ignore gracefully (service unavailable / unimplemented)
                        }
                    }

                    foreach (var s in dto.Suppliers)
                        s.SupplierName = nameMap.TryGetValue(s.SupplierId, out var nm) ? nm : null;
                }
            }


            // ✅ Make sure to return the DTO
            return dto;
        }


        public async Task<string> GetBaseDirectoryAsync(CancellationToken ct = default)
        {
            const string query = @"
            SELECT Description AS BaseDirectory  
                FROM Inventory.MiscTypeMaster 
                WHERE MiscTypeCode='ItemImage'  
                AND IsDeleted=0
            ";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(query);
            return result;
        }
        public async Task<bool> RemoveImageReferenceAsync(string imageName)
        {
            var asset = await _db.ItemMaster.FirstOrDefaultAsync(x => x.ItemImage == imageName);
            if (asset == null)
            {
                return false;  // Asset not found
            }
            asset.ItemImage = null;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateItemImageAsync(int ItemId, string imageName,CancellationToken ct = default)
        {
            var asset = await _db.ItemMaster.FindAsync(ItemId);
            if (asset == null)
            {
                return false;  // Asset not found
            }
            asset.ItemImage = imageName;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<string?> GetLatestItemCode(int itemGroupId, int itemCategoryId, CancellationToken ct = default)
        {             
            var parameters = new DynamicParameters();
            parameters.Add("@GroupId", itemGroupId);
            parameters.Add("@CategoryId", itemCategoryId);         
            var newAssetCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "dbo.Sp_GetItemCode", 
                parameters, 
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120);
            return newAssetCode;         
        }
        public async Task<List<string>> GetCandidateItemNamesAsync(string normalizedPrefix, int take = 200, CancellationToken ct = default)
        {
            // Use first 3 chars of normalized input as lightweight prefilter
       /*      if (string.IsNullOrWhiteSpace(normalizedPrefix)) return new();
            var p = normalizedPrefix.Length >= 3 ? normalizedPrefix[..3] : normalizedPrefix; */

            var hint = normalizedPrefix?.Replace("%", "").Replace("_", "") ?? "";
            return await _db.ItemMaster
                .AsNoTracking()
                .Where(i => i.IsDeleted == BaseEntity.IsDelete.NotDeleted && i.ItemName != null)
                .Where(i => EF.Functions.Like(
                    EF.Functions.Collate(i.ItemName!, "Latin1_General_CI_AI"), $"%{hint}%"))
                .OrderByDescending(i => i.Id)
                .Select(i => i.ItemName!)
                .Take(take <= 0 ? 100 : take)
                .ToListAsync(ct);

       /*      return await _db.ItemMaster
                .Where(x => x.IsDeleted == BaseEntity.IsDelete.NotDeleted &&
                            EF.Functions.Like(x.ItemName, $"%{p}%"))
                .OrderBy(x => x.ItemName)
                .Select(x => x.ItemName)
                .Take(take)
                .ToListAsync(ct); */
        }

        public async Task<List<GetItemAutoCompleteDto>> GetItemAutoCompleteAsync(string searchPattern)
        {
            searchPattern = searchPattern ?? string.Empty;
            const string query = @"
             SELECT Id, ItemName,ItemCode,ParentItemId
            FROM Inventory.ItemMaster             
            WHERE IsDeleted = 0 
            AND ItemName LIKE @SearchPattern";
            var parameters = new
            {
                SearchPattern = $"%{searchPattern}%"
            };
            var items = await _dbConnection.QueryAsync<GetItemAutoCompleteDto>(query, parameters);
            return items.ToList();
        }
    }
}
