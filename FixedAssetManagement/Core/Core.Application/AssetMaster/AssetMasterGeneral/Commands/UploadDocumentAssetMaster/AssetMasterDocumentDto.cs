using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadDocumentAssetMaster
{
    public class AssetMasterDocumentDto : IMapFrom<AssetMasterGenerals>
    {
        public string? AssetDocument { get; set; }
        public string? AssetDocumentBase64 { get; set; }
    }
}