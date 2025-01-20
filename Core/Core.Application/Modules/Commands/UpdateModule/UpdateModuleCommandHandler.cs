using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using FluentValidation;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.IModule;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;

namespace Core.Application.Modules.Commands.UpdateModule
{
    public class UpdateModuleCommandHandler: IRequestHandler<UpdateModuleCommand,ApiResponseDTO<bool>>
    {
    private readonly IModuleCommandRepository _moduleRepository;
    private readonly IModuleQueryRepository _moduleQueryRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator; 
    private readonly ILogger<UpdateModuleCommandHandler> _logger;


    public UpdateModuleCommandHandler(IModuleCommandRepository moduleRepository, IMapper mapper, IModuleQueryRepository moduleQueryRepository, IMediator mediator,ILogger<UpdateModuleCommandHandler> logger)
    {
        _moduleRepository = moduleRepository;
        _mapper = mapper;
        _moduleQueryRepository = moduleQueryRepository;
        _mediator = mediator;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApiResponseDTO<bool>> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
            if (request == null)
            {
                _logger.LogError("UpdateModuleCommand request is null.");
                throw new ArgumentNullException(nameof(request));
            }
        _logger.LogInformation("Starting module update process for ModuleId: {ModuleId}", request.ModuleId);


        var module = await _moduleQueryRepository.GetModuleByIdAsync(request.ModuleId);
        if (module == null || module.IsDeleted)
        {
                _logger.LogWarning("Module with ID {ModuleId} not found or has been deleted.", request.ModuleId);
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "Module not found or has been deleted."
                };
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
    
        //Publish Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: OldModuleName,
                    actionName: module.ModuleName,
                    details: $"Module '{OldModuleName}' was updated to '{module.ModuleName}'. Added menus: {string.Join(", ", menusToAdd)}. Removed menus: {string.Join(", ", menusToRemove)}.",
                    module:"Module"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);

                await _moduleRepository.SaveChangesAsync();
          _logger.LogInformation("Module with ID {ModuleId} successfully updated.", request.ModuleId);

            return new ApiResponseDTO<bool>
            {
                IsSuccess = true,
                Message = "Module updated successfully.",
                Data = true
            };
    }
}
}