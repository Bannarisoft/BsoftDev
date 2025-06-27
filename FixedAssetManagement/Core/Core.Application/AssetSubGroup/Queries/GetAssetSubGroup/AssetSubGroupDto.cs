using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetSubGroup.Queries.GetAssetSubGroup
{
    public class AssetSubGroupDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? SubGroupName { get; set; }
        public int SortOrder { get; set; }
        public int GroupId { get; set; }
        public Status IsActive { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string? CreatedByName { get; set; }
    }
}