using AutoMapper;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Item.ItemDetail.Queries.GetItemAutoComplete;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Item.ItemDetail.Queries.GetItemAutoComplete
{
    public class GetItemAutoCompleteQueryHandler
        : IRequestHandler<GetItemAutoCompleteQuery, List<GetItemAutoCompleteDto>>
    {
        private readonly IItemQueryRepository _itemQueryRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetItemAutoCompleteQueryHandler(
            IItemQueryRepository itemQueryRepository,
            IMediator mediator,
            IMapper mapper)
        {
            _itemQueryRepository = itemQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<List<GetItemAutoCompleteDto>> Handle(
            GetItemAutoCompleteQuery request,
            CancellationToken cancellationToken)
        {
            // If your repo supports ct, pass it: GetItemAutoCompleteAsync(pattern, cancellationToken)
            var result = await _itemQueryRepository.GetItemAutoCompleteAsync(request.SearchPattern ?? string.Empty);

            // ✅ map to the correct DTO type
            var items = _mapper.Map<List<GetItemAutoCompleteDto>>(result);

            // Domain event (optional)
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAutocomplete",
                actionCode: nameof(GetItemAutoCompleteQueryHandler),
                actionName: items.Count.ToString(),
                details: "Item autocomplete fetched.",
                module: "ItemMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            return items;
        }
    }
}
