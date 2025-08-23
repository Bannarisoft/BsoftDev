namespace Contracts.Dtos.Warehouse
{
    public class BinDto
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int RackId { get; set; }
        public string BinCode { get; set; }
        public string BinName { get; set; }
        public double BinCapacity { get; set; }
        public int CapacityUOMId { get; set; }
        public int IsActive { get; set; }
    }
}
