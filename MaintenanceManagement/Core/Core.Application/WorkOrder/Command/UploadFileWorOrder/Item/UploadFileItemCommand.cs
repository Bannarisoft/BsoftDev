

using Core.Application.Common.HttpResponse;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.WorkOrder.Command.UploadFileWorOrder.Item
{
    public class UploadFileItemCommand : IRequest<ApiResponseDTO<ItemImageDto>>
    {
         public IFormFile? File { get; set; }       
    }
}