namespace Core.Application.Item.PutAway.Queries.GetPutAwayTargets
{
    public sealed class PutAwayTargetLookupDto
    {
        public int Id { get; set; }           // target row id (Rack.Id, Bin.Id, OpenSpace.Id)
        public string Code { get; set; } = ""; // display code (e.g., "R-01", "BIN-001")
        public string Name { get; set; } = ""; // friendly name (e.g., "Rack Aisle 1")
    }
}
