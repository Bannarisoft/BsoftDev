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

namespace Core.Application.Modules.Commands.CreateModule
{
    public class CreateModuleCommandHandler : IRequestHandler<CreateModuleCommand, int>
    {
        private readonly IModuleCommandRepository _moduleRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

    public CreateModuleCommandHandler(IModuleCommandRepository moduleRepository, IMapper mapper, IMediator mediator)
    {
        _moduleRepository = moduleRepository;
        _mapper = mapper;
        _mediator = mediator;    

    }
    public async Task<int> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
    {
        // Validate inputs
        if (string.IsNullOrEmpty(request.ModuleName))
            throw new ValidationException("Module name is required.");
            
        if (request.Menus == null || !request.Menus.Any())
            throw new ValidationException("At least one menu is required.");

        // Map to domain model
        var module = new Domain.Entities.Modules
        {
            ModuleName = request.ModuleName,
            Menus = request.Menus.Select(menuName => new Menu { MenuName = menuName }).ToList()
        };

        //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: request.ModuleName,
                actionName: request.ModuleName,
                details: $"Modules '{request.ModuleName}' was created. ModuleName: {request.ModuleName}",
                module:"Modules"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
        await _moduleRepository.AddModuleAsync(module);
        await _moduleRepository.SaveChangesAsync();

        return module.Id;
    }
    }
}