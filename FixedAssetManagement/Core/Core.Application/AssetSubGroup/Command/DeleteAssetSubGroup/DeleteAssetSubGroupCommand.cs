using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetSubGroup.Command.DeleteAssetSubGroup
{
    public class DeleteAssetSubGroupCommand : IRequest<ApiResponseDTO<int>> 
    {
        public int Id { get; set; }
    }
}