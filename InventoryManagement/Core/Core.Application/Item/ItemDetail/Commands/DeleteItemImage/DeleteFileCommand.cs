using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Item.ItemDetail.Commands.DeleteItemImage
{
    public class DeleteFileCommand : IRequest<bool>
    {        public string? imagePath { get; set; }       
    }
}