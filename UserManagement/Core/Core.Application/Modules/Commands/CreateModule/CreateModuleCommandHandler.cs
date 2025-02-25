using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using FluentValidation;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IModule;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;

namespace Core.Application.Modules.Commands.CreateModule
{
    public class CreateModuleCommandHandler : IRequestHandler<CreateModuleCommand, ApiResponseDTO<int>>
    {
        private readonly IModuleCommandRepository _moduleRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateModuleCommandHandler> _logger;


    public CreateModuleCommandHandler(IModuleCommandRepository moduleRepository, IMapper mapper, IMediator mediator,ILogger<CreateModuleCommandHandler> logger)
    {
        _moduleRepository = moduleRepository;
        _mapper = mapper;
        _mediator = mediator;    
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    }
    public async Task<ApiResponseDTO<int>> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
            {
                _logger.LogError("CreateModuleCommand request is null.");
                throw new ArgumentNullException(nameof(request));
            }

         _logger.LogInformation("Starting module creation process for ModuleName: {ModuleName}", request.ModuleName);
        // Validate inputs
        if (string.IsNullOrEmpty(request.ModuleName))
        {
            _logger.LogWarning("Validation failed: Module name is required.");
            throw new ValidationException("Module name is required.");
        }    
        if (request.Menus == null || !request.Menus.Any())
        {   
            _logger.LogWarning("Validation failed: At least one menu is required.");
            throw new ValidationException("At least one menu is required.");
        }
        // Map to domain model
        var module = new Domain.Entities.Modules
        {
            ModuleName = request.ModuleName,
            Menus = request.Menus.Select(menuName => new Domain.Entities.Menu { MenuName = menuName }).ToList()
        };
        await _moduleRepository.AddModuleAsync(module);
        await _moduleRepository.SaveChangesAsync();

        //Publish Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: request.ModuleName,
                actionName: request.ModuleName,
                details: $"Module '{module.ModuleName}' was created with {module.Menus.Count} menus.",
                module:"Modules"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("Module successfully created with ID: {ModuleId}", module.Id);


            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "Module created successfully.",
                Data = module.Id
            };
    }
    }
}