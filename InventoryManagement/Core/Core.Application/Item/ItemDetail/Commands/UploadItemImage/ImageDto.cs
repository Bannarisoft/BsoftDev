
using Core.Application.Common.Mappings;
using Core.Domain.Entities.Item.ItemDetail;

namespace Core.Application.Item.ItemDetail.Commands.UploadItemImage
{
    public class ImageDto : IMapFrom<ItemMaster>
    {
        public string? AssetImage { get; set; }
        public string? AssetImageBase64 { get; set; } 

    }
}