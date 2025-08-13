using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Domain.Common;
using Core.Domain.Entities.Item.ItemDetail.Variant;
using Dapper;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Queries
{
    public sealed class ItemQueryRepository : IItemQueryRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IDbConnection _dbConnection;
        public ItemQueryRepository(IDbConnection dbConnection, ApplicationDbContext db)
        {
            _db = db;
            _dbConnection = dbConnection;            
        }

        public async Task<(List<ItemListDto> Items, int TotalCount)> GetAllAsync(int page, int size, string? search, bool onlyActive, CancellationToken ct = default)
        {
            var q = _db.ItemMaster.AsNoTracking()
                .Where(x => x.IsDeleted == BaseEntity.IsDelete.NotDeleted);

            if (onlyActive) q = q.Where(x => x.IsActive == BaseEntity.Status.Active);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                q = q.Where(x => x.ItemCode.Contains(term) || x.ItemName.Contains(term));
            }

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
                    UnitId = x.UnitId
                })
                .ToListAsync(ct);

            return (list, total);
        }

        public async Task<ItemDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
                return await _db.ItemMaster
                .AsNoTracking()
                .Where(i => i.Id == id && i.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .Select(i => new ItemDetailsDto
                {
                    // base
                    Id                   = i.Id,
                    UnitId               = i.UnitId,
                    ItemCode             = i.ItemCode,
                    ItemName             = i.ItemName,
                    HSNId                = i.HSNId,
                    HSNCode = i.HSNMaster != null ? i.HSNMaster.HSNCode : null,
                    ItemGroupId          = i.ItemGroupId,
                    ItemGroupName = i.ItemGroup != null ? i.ItemGroup.ItemGroupName : null,
                    ItemCategoryId       = i.ItemCategoryId,
                    StockUomId           = i.StockUomId,
                    ItemClassificationId = i.ItemClassificationId,
                    Description          = i.Description,
                    ValidFrom            = i.ValidFrom,
                    XPlantMaterialStatusId = i.XPlantMaterialStatusId,
                    IsStockItem          = i.IsStockItem,
                    MaintainStock        = i.MaintainStock,
                    HasVariants          = i.HasVariants,
                    ParentItemId         = i.ParentItemId,
                    ItemImage            = i.ItemImage, // assumes the column exists on ItemMaster

                    // tabs (1-1)
                    Purchase = i.Purchase == null ? null : new ItemPurchaseDto
                    {
                        PurchaseUomId       = i.Purchase.PurchaseUomId,
                        LeadTimeDays        = i.Purchase.LeadTimeDays,
                        SafetyStock         = i.Purchase.SafetyStock,
                        GrProcessingTimeDays= i.Purchase.GrProcessingTimeDays,
                        AutomaticPo         = i.Purchase.AutomaticPo,
                        OriginCountryId     = i.Purchase.OriginCountryId,
                        TariffNumber        = i.Purchase.TariffNumber
                    },
                    Inventory = i.Inventory == null ? null : new ItemInventoryDto
                    {
                        Weight                    = i.Inventory.Weight,
                        WeightUomId               = i.Inventory.WeightUomId,
                        DefaultMaterialRequestTypeId = i.Inventory.DefaultMaterialRequestTypeId,
                        ValuationMethodId         = i.Inventory.ValuationMethodId,
                        ShelfLife                 = i.Inventory.ShelfLife,
                        UpperTolerance            = i.Inventory.UpperTolerance,
                        LowerTolerance            = i.Inventory.LowerTolerance,
                        BatchNumberSeries         = i.Inventory.BatchNumberSeries,
                        SerialNumberSeries        = i.Inventory.SerialNumberSeries,
                        ReorderLevel              = i.Inventory.ReorderLevel,
                        ReorderQty                = i.Inventory.ReorderQty,
                        RequestTypeId             = i.Inventory.RequestTypeId,
                        AllowNegativeStock        = i.Inventory.AllowNegativeStock,
                        BatchManagement           = i.Inventory.BatchManagement,
                        ApplyBatchNumber          = i.Inventory.ApplyBatchNumber
                    },
                    Quality = i.Quality == null ? null : new ItemQualityDto
                    {
                        InspectionTemplateId        = i.Quality.InspectionTemplateId,
                        CertificateTypeId           = i.Quality.CertificateTypeId,
                        InspLotProcessingTime       = i.Quality.InspLotProcessingTime,
                        InspectionRequired          = i.Quality.InspectionRequired,
                        QualityInspectionFree       = i.Quality.QualityInspectionFree,
                        IsCertificateRequiredFromSupplier = i.Quality.IsCertificateRequiredFromSupplier
                    },

                    // collections
                    Suppliers = i.Suppliers                        
                        .OrderBy(s => s.SupplierId)
                        .Select(s => new ItemSupplierDto
                        {
                            SupplierId    = s.SupplierId,
                            UnitId        = s.UnitId,
                            SupplierPartNo= s.SupplierPartNo
                        }).ToList(),

                    Manufacture = i.Manufacture                        
                        .OrderBy(m => m.UnitId)
                        .Select(m => new ItemManufactureDto
                        {
                            UnitId             = m.UnitId,
                            ManufacturingTypeId= m.ManufacturingTypeId
                        }).ToList(),

                    Uoms = i.ItemUOMs
                        .Select(u => new ItemUomDto
                        {
                            ConversionUOMId = u.ConversionUOMId,
                            ConversionRate  = u.ConversionRate
                        }).ToList(),

                    // variant values (same table for template or child)
                    VariantValues = _db.Set<ItemVariantValue>()
                        .Where(v => v.ItemId == i.Id)
                        .OrderBy(v => v.AttributeId)
                        .Select(v => new VariantValueDto
                        {
                            AttributeId = v.AttributeId,
                            OptionValue = v.OptionValue
                        }).ToList()
                })
                .FirstOrDefaultAsync(ct);
        }
        public async Task<string> GetBaseDirectoryAsync(CancellationToken ct = default)
        {
            const string query = @"
            SELECT Description AS BaseDirectory  
                FROM Maintenance.MiscTypeMaster 
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
            if (string.IsNullOrWhiteSpace(normalizedPrefix)) return new();
            var p = normalizedPrefix.Length >= 3 ? normalizedPrefix[..3] : normalizedPrefix;

            return await _db.ItemMaster
                .Where(x => x.IsDeleted == BaseEntity.IsDelete.NotDeleted &&
                            EF.Functions.Like(x.ItemName, $"%{p}%"))
                .OrderBy(x => x.ItemName)
                .Select(x => x.ItemName)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
