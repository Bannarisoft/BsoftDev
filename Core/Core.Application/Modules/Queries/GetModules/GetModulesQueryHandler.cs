using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IModule;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Modules.Queries.GetModules
{
    public class GetModulesQueryHandler : IRequestHandler<GetModulesQuery, List<ModuleDto>>
    {
    private readonly IModuleQueryRepository _moduleRepository;
    private readonly IMapper _mapper;
        private readonly IMediator _mediator; 


    public GetModulesQueryHandler(IModuleQueryRepository moduleRepository, IMapper mapper, IMediator mediator)
    {
        _moduleRepository = moduleRepository;
        _mapper = mapper;
        _mediator = mediator;

    }

    public async Task<List<ModuleDto>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
    {
        var modules = await _moduleRepository.GetAllModulesAsync();
        //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"Module details was fetched.",
                    module:"Module"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
        return _mapper.Map<List<ModuleDto>>(modules);
    }
    }
}