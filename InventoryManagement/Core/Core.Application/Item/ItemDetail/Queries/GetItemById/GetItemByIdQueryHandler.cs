using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Core.Application.Common.Interfaces.Item.ItemDetail;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using AutoMapper;

namespace Core.Application.Item.ItemDetail.Queries.GetItemById
{
     public sealed class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto?>
    {
        private readonly IItemQueryRepository _repo;
        public GetItemByIdQueryHandler(IItemQueryRepository repo) => _repo = repo;

        public Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken ct)
            => _repo.GetByIdAsync(request.Id, ct); // repo already returns ItemDto?
    }
}
