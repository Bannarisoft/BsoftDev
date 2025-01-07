using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IModule;
using MediatR;

namespace Core.Application.Modules.Queries.GetModules
{
    public class GetModulesQueryHandler : IRequestHandler<GetModulesQuery, List<ModuleDto>>
    {
    private readonly IModuleQueryRepository _moduleRepository;
    private readonly IMapper _mapper;

    public GetModulesQueryHandler(IModuleQueryRepository moduleRepository, IMapper mapper)
    {
        _moduleRepository = moduleRepository;
        _mapper = mapper;
    }

    public async Task<List<ModuleDto>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
    {
        var modules = await _moduleRepository.GetAllModulesAsync();
        return _mapper.Map<List<ModuleDto>>(modules);
    }
    }
}