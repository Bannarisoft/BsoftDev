using AutoMapper;
using BSOFT.Application.Common.Interfaces;
using MediatR;

namespace BSOFT.Application.Modules.Queries.GetModules
{
    public class GetModulesQueryHandler : IRequestHandler<GetModulesQuery, List<ModuleDto>>
    {
    private readonly IModuleRepository _moduleRepository;
    private readonly IMapper _mapper;

    public GetModulesQueryHandler(IModuleRepository moduleRepository, IMapper mapper)
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