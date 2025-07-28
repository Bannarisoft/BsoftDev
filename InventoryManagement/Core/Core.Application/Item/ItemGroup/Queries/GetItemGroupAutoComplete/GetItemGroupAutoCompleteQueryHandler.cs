
using AutoMapper;
using Core.Application.Common.Interfaces.Item.ItemGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Item.ItemGroup.Queries.GetItemGroupAutoComplete
{
    public class GetItemGroupAutoCompleteQueryHandler : IRequestHandler<GetItemGroupAutoCompleteQuery,List<ItemGroupAutoCompleteDto>>
    {
        private readonly IItemGroupQueryRepository _itemGroupQueryRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public GetItemGroupAutoCompleteQueryHandler(IItemGroupQueryRepository itemGroupQueryRepository, IMediator mediator, IMapper mapper)
        {
            _itemGroupQueryRepository = itemGroupQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<List<ItemGroupAutoCompleteDto>> Handle(GetItemGroupAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _itemGroupQueryRepository.GetItemGroupAutoCompleteAsync(request.SearchPattern ?? string.Empty);
            var itemGroup = _mapper.Map<List<ItemGroupAutoCompleteDto>>(result);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "GetNotificationConfigAutoCompleteQueryHandler",        
                actionName: itemGroup.Count.ToString(),
                details: $"Notification Config details was fetched.",
                module:"NotificationConfig"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return itemGroup;
        }
    }
}