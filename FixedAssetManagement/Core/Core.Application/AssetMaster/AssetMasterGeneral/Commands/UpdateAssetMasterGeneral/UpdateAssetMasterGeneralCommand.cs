
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral
{
    public class UpdateAssetMasterGeneralCommand : IRequest<ApiResponseDTO<bool>>     
    {
        public AssetMasterUpdateDto? AssetMaster { get; set; }   
    }
}