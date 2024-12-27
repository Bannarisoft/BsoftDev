using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using FluentValidation;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Modules.Commands.CreateModule
{
    public class CreateModuleCommandHandler : IRequestHandler<CreateModuleCommand, int>
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IMapper _mapper;
    public CreateModuleCommandHandler(IModuleRepository moduleRepository, IMapper mapper)
    {
        _moduleRepository = moduleRepository;
        _mapper = mapper;
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

        await _moduleRepository.AddModuleAsync(module);
        await _moduleRepository.SaveChangesAsync();

        return module.Id;
    }
    }
}