using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.Item.ItemGroup;
using Core.Application.Item.ItemGroup.Commands.CreateItemGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Item.ItemGroup.Commands.CreateItemGroup
{
    public class CreateItemGroupCommandHandler : IRequestHandler<CreateItemGroupCommand, int>
    {
        private readonly IItemGroupCommandRepository _itemGroupRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CreateItemGroupCommandHandler(IItemGroupCommandRepository itemGroupRepository, IMediator mediator, IMapper mapper)
        {
            _itemGroupRepository = itemGroupRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateItemGroupCommand request, CancellationToken cancellationToken)
        {
            var itemGroup = _mapper.Map<Domain.Entities.Item.ItemGroup>(request);
            var result = await _itemGroupRepository.CreateAsync(itemGroup);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: itemGroup.Id.ToString(),
                actionName: itemGroup.ItemGroupName ?? "",
                details: $"Item Group details was created",
                module: "itemGroup");
            await _mediator.Publish(domainEvent, cancellationToken);

            return result > 0 ? result : throw new ExceptionRules("ItemGroup Creation Failed.");
        }
    }

}
