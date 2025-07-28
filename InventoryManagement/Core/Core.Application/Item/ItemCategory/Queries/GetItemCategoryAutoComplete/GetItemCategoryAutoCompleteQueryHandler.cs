
using AutoMapper;
using Core.Application.Common.Interfaces.Item.ItemCategory;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Item.ItemCategory.Queries.GetItemCategoryAutoComplete
{
    public class GetItemCategoryAutoCompleteQueryHandler : IRequestHandler<GetItemCategoryAutoCompleteQuery,List<ItemCategoryAutoCompleteDto>>
    {
        private readonly IItemCategoryQueryRepository _itemCategoryQueryRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public GetItemCategoryAutoCompleteQueryHandler(IItemCategoryQueryRepository itemCategoryQueryRepository, IMediator mediator, IMapper mapper)
        {
            _itemCategoryQueryRepository = itemCategoryQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<List<ItemCategoryAutoCompleteDto>> Handle(GetItemCategoryAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _itemCategoryQueryRepository.GetItemCategoryAutoCompleteAsync(request.SearchPattern ?? string.Empty);
            var itemCategory = _mapper.Map<List<ItemCategoryAutoCompleteDto>>(result);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "GetNotificationConfigAutoCompleteQueryHandler",        
                actionName: itemCategory.Count.ToString(),
                details: $"Notification Config details was fetched.",
                module:"NotificationConfig"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return itemCategory;
        }
    }
}