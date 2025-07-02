using Core.Application.AssetSubGroup.Queries.GetAssetSubGroup;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetSubGroup.Queries.GetAssetGroupById
{
    public class GetGroupByIdQuery : IRequest<ApiResponseDTO<List<AssetSubGroupDto>>>
    {
        public int GroupId { get; set; }        
    }
}