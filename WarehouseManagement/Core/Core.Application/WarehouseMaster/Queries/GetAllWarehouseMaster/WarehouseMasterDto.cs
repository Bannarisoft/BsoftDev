using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.WarehouseMaster.GetAllWarehouseMaster
{
    public class WarehouseMasterDto
    {
        public int Id { get; set; }
        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public int? ParentWarehouseId { get; set; }
        public bool IsGroup { get; set; }
        public bool IsVirtualWarehouse { get; set; }
        public int WarehouseTypeId { get; set; }
        public string? WarehouseTypeName { get; set; }
        public int StorageTypeId { get; set; }
        public string? StorageTypeName { get; set; }
        public int AreaTypeId { get; set; }
        public string? AreaTypeName { get; set; }
        public int OperationTypeId { get; set; }
        public string? OperationTypeName { get; set; }
        public int CapacityUOMId { get; set; }
        public string? CapacityUOMName { get; set; }
        public int? AccountId { get; set; }
        public string? ContactPersonName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public int CityId { get; set; }
        public string? CityName { get; set; }
        public int StateId { get; set; }
        public string? StateName { get; set; }
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
        public string Pincode { get; set; } = string.Empty;
        public bool IsScrapWarehouse { get; set; }
        public bool IsTransitWarehouse { get; set; }
        public decimal MaxCapacity { get; set; }
        public bool IsDefaultStockEntry { get; set; }        
        public byte IsActive { get; set; }
    }
}