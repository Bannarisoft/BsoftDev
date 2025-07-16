using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMenu;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Menu.Queries.GetChildMenuByModule
{
    public class GetChildMenuByModuleQueryHandler : IRequestHandler<GetChildMenuByModuleQuery, ApiResponseDTO<List<ChildMenuDTO>>>
    {
          private readonly IMenuQuery _menuQuery;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetChildMenuByModuleQueryHandler(IMenuQuery menuQuery, IMapper mapper, IMediator mediator)
        {
            _menuQuery = menuQuery;
            _mapper = mapper;
            _mediator = mediator;   
        }
        public async Task<ApiResponseDTO<List<ChildMenuDTO>>> Handle(GetChildMenuByModuleQuery request, CancellationToken cancellationToken)
        {
             var menus = await _menuQuery.GetChildMenus(request.ParentId);
            var menusList = _mapper.Map<List<ChildMenuDTO>>(menus);

             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetChildMenuByParent",
                    actionCode: "",        
                    actionName: "",
                    details: $"Child Menu details was fetched.",
                    module:"Child Menu"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<ChildMenuDTO>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = menusList 
                };
        }
    }
}