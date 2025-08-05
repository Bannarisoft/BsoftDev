using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.WarehouseMaster.GetAllWarehouseMaster
{
    public class WarehouseMasterDto
    {
        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public int? ParentWarehouseId { get; set; }
        public bool IsGroup { get; set; }
        public int WarehouseTypeId { get; set; }

        public string? WarehouseTypeName { get; set;}
        public int StorageTypeId { get; set; }
        public string? StorageTypeName { get; set; }
        public int CapacityUOMId { get; set; }
        public int? AccountId { get; set; }
        public string? ContactPersonName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string Pincode { get; set; } = string.Empty;
        public bool IsScrapWarehouse { get; set; }
        public bool IsTransitWarehouse { get; set; }
        public decimal MaxCapacity { get; set; }
        public bool IsDefaultStockEntry { get; set; }
    }
}