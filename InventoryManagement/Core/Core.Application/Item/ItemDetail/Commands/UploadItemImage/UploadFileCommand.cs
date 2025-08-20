using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Item.ItemDetail.Commands.UploadItemImage
{
    public class UploadFileCommand : IRequest<ImageDto>
    {
        public IFormFile? File { get; set; }
    }
}
