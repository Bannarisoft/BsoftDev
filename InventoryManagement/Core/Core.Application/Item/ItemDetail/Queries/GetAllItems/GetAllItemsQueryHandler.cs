using MediatR;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;

namespace Core.Application.Item.ItemDetail.Queries.GetAllItems
{
    public sealed class GetAllItemsQueryHandler
        : IRequestHandler<GetAllItemsQuery, (List<ItemListDto> Items, int TotalCount)>
    {
        private readonly IItemQueryRepository _repo;
        public GetAllItemsQueryHandler(IItemQueryRepository repo) => _repo = repo;

        public Task<(List<ItemListDto> Items, int TotalCount)> Handle(GetAllItemsQuery request, CancellationToken ct)
            => _repo.GetAllAsync(request.PageNumber, request.PageSize, request.SearchTerm, request.OnlyActive, ct);
    }
}
