using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IItem;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Item.ItemMaster.Queries
{
    public class GetItemMasterQueryHandler :  IRequestHandler<GetItemMasterQuery,List<GetItemMasterDto>>
    {
         private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IItemQueryRepository _itemQueryRepository;

        public GetItemMasterQueryHandler(IMapper mapper, IMediator mediator, IItemQueryRepository itemQueryRepository)
        {
            _mapper = mapper;
            _mediator = mediator;
            _itemQueryRepository = itemQueryRepository;
        }

        public async Task<List<GetItemMasterDto>> Handle(GetItemMasterQuery request, CancellationToken cancellationToken)
        {
            var result = await _itemQueryRepository.GetItemMasters(request.OldUnitId, request.Grpcode, request.ItemCode, request.ItemName);
            var itemmaster  = _mapper.Map<List<GetItemMasterDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "GetItemMasterQuery",        
                    actionName: "ItemMaster",
                    details: $"ItemMaster details was fetched.",
                    module:"ItemMaster"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return itemmaster;
        }
    }
}