using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Domain.Common;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Queries
{
    public sealed class ItemQueryRepository : IItemQueryRepository
    {
        private readonly ApplicationDbContext _db;
        public ItemQueryRepository(ApplicationDbContext db) => _db = db;

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

        public Task<ItemDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return _db.ItemMaster.AsNoTracking()
                .Where(x => x.Id == id && x.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .Select(x => new ItemDto
                {
                    UnitId = x.UnitId,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    HSNCode = x.HSNCode,
                    ItemGroupId = x.ItemGroupId,
                    ItemCategoryId = x.ItemCategoryId,
                    DefaultUomId = x.DefaultUomId,
                    ItemClassificationId = x.ItemClassificationId,
                    Description = x.Description,
                    ValidFrom = x.ValidFrom,
                    XPlantMaterialStatusId = x.XPlantMaterialStatusId,
                    DepartmentId = x.DepartmentId,
                    IsStockItem = x.IsStockItem,
                    MaintainStock = x.MaintainStock,
                    HasVariants = x.HasVariants,
                    ParentItemId = x.ParentItemId,

                    Purchase = x.Purchase == null ? null : new ItemPurchaseDto
                    {
                        PurchaseUomId = x.Purchase.PurchaseUomId,
                        LeadTimeDays = x.Purchase.LeadTimeDays,
                        SafetyStock = x.Purchase.SafetyStock,
                        GrProcessingTimeDays = x.Purchase.GrProcessingTimeDays,
                        PurchaseRate = x.Purchase.PurchaseRate,
                        AutomaticPo = x.Purchase.AutomaticPo,
                        OriginCountryId = x.Purchase.OriginCountryId,
                        TariffNumber = x.Purchase.TariffNumber
                    },

                    Inventory = x.Inventory == null ? null : new ItemInventoryDto
                    {
                        Weight = x.Inventory.Weight,
                        WeightUomId = x.Inventory.WeightUomId,
                        DefaultMaterialRequestTypeId = x.Inventory.DefaultMaterialRequestTypeId,
                        ValuationMethodId = x.Inventory.ValuationMethodId,
                        ShelfLife = x.Inventory.ShelfLife,
                        UpperTolerance = x.Inventory.UpperTolerance,
                        LowerTolerance = x.Inventory.LowerTolerance,
                        BatchNumberSeries = x.Inventory.BatchNumberSeries,
                        SerialNumberSeries = x.Inventory.SerialNumberSeries,
                        ReorderLevel = x.Inventory.ReorderLevel,
                        ReorderQty = x.Inventory.ReorderQty,
                        RequestTypeId = x.Inventory.RequestTypeId,
                        AllowNegativeStock = x.Inventory.AllowNegativeStock,
                        BatchManagement = x.Inventory.BatchManagement,
                        ApplyBatchNumber = x.Inventory.ApplyBatchNumber
                    },

                    Quality = x.Quality == null ? null : new ItemQualityDto
                    {
                        InspectionTemplateId = x.Quality.InspectionTemplateId,
                        CertificateTypeId = x.Quality.CertificateTypeId,
                        InspLotProcessingTime = x.Quality.InspLotProcessingTime,
                        InspectionRequired = x.Quality.InspectionRequired,
                        QualityInspectionFree = x.Quality.QualityInspectionFree,
                        IsCertificateRequiredFromSupplier = x.Quality.IsCertificateRequiredFromSupplier
                    },

                    Suppliers = x.Suppliers
                        .Where(s => s.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                        .Select(s => new ItemSupplierDto { SupplierId = s.SupplierId, UnitId = s.UnitId, SupplierPartNo = s.SupplierPartNo })
                        .ToList(),

                    Manufacture = x.Manufacture
                        .Where(m => m.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                        .Select(m => new ItemManufactureDto { UnitId = m.UnitId, ManufacturingTypeId = m.ManufacturingTypeId })
                        .ToList(),

                    VariantDefs = x.VariantDefs == null ? null :
                        x.VariantDefs.Select(v => new VariantDefDto { AttributeId = v.AttributeId, OptionIds = v.Options.Select(o => o.OptionId).ToList() }).ToList(),
                    
                    Uoms = _db.ItemUOMs
                        .Where(u => u.ItemId == x.Id)
                        .Select(u => new ItemUomDto
                        {
                            BaseUOMId = u.BaseUOMId,
                            ConversionUOMId = u.ConversionUOMId,
                            ConversionRate = u.ConversionRate
                        }).ToList()
                })
                .FirstOrDefaultAsync(ct);
        }               
    }
}
