using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById
{
    public class GetAssetMasterGeneralByIdQuery : IRequest<AssetMasterDTO>
    {
        public int Id { get; set; }
    }
}