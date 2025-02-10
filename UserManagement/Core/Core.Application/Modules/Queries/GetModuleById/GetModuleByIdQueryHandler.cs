using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IModule;
using Core.Application.Modules.Queries.GetModules;
using Core.Application.RoleEntitlements.Queries.GetRoleEntitlements;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Modules.Queries.GetModuleById
{
    public class GetModuleByIdQueryHandler : IRequestHandler<GetModuleByIdQuery,ApiResponseDTO<ModuleByIdDto>>
    {
        private readonly IModuleQueryRepository _moduleQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
    
    public GetModuleByIdQueryHandler(IModuleQueryRepository moduleQueryRepository, IMapper mapper, IMediator mediator)
    {
        _moduleQueryRepository = moduleQueryRepository;
        _mapper = mapper;
        _mediator = mediator;
        
    }

        public async Task<ApiResponseDTO<ModuleByIdDto>> Handle(GetModuleByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _moduleQueryRepository.GetModuleByIdAsync(request.Id);
             if (result == null)
            {
                return new ApiResponseDTO<ModuleByIdDto> { IsSuccess = false, Message = "Module not found", Data = null };
            }
            var modules = _mapper.Map<ModuleByIdDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "MOD_FETCH",        
                    actionName: "Fetch Module",
                    details: $"Module details for ModuleName {modules.ModuleName} were fetched.",
                    module:"Modules"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<ModuleByIdDto> { IsSuccess = true, Message = "Success", Data = modules };
        }
    }
}