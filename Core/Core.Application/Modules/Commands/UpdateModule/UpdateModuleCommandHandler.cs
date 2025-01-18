using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using FluentValidation;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.IModule;
using Core.Domain.Events;

namespace Core.Application.Modules.Commands.UpdateModule
{
    public class UpdateModuleCommandHandler: IRequestHandler<UpdateModuleCommand>
    {
    private readonly IModuleCommandRepository _moduleRepository;
    private readonly IModuleQueryRepository _moduleQueryRepository;
    private readonly IMapper _mapper;
        private readonly IMediator _mediator; 


    public UpdateModuleCommandHandler(IModuleCommandRepository moduleRepository, IMapper mapper, IModuleQueryRepository moduleQueryRepository, IMediator mediator)
    {
        _moduleRepository = moduleRepository;
        _mapper = mapper;
        _moduleQueryRepository = moduleQueryRepository;
        _mediator = mediator;

    }

    public async Task Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
    var module = await _moduleQueryRepository.GetModuleByIdAsync(request.ModuleId);
    if (module == null || module.IsDeleted)
    {
        throw new ValidationException("Module not found or has been deleted.");
    }

    // Update module name
    module.ModuleName = request.ModuleName;

    // Update menus
    var existingMenus = module.Menus.Select(m => m.MenuName).ToList();
    var menusToAdd = request.Menus.Except(existingMenus).ToList();
    var menusToRemove = existingMenus.Except(request.Menus).ToList();

    foreach (var menuName in menusToAdd)
    {
        module.Menus.Add(new Menu { MenuName = menuName });
    }

    module.Menus = module.Menus.Where(m => !menusToRemove.Contains(m.MenuName)).ToList();

                var OldModuleName = module.ModuleName;
            module.ModuleName = request.ModuleName;
    
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: module.ModuleName,
                    actionName: module.ModuleName,
                    details: $"Module '{OldModuleName}' was updated to '{module.ModuleName}'.  ModuleName: {module.ModuleName}",
                    module:"Module"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);

    await _moduleRepository.SaveChangesAsync();
    }
    }
}