using Core.Application.AssetSubGroup.Queries.GetAssetSubGroup;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetSubGroup.Queries.GetAssetSubGroupById
{
    public class GetAssetSubGroupByIdQuery : IRequest<AssetSubGroupDto>
    {        
        public int Id { get; set; }
    }
}