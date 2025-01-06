using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using FluentValidation;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.IModule;

namespace BSOFT.Application.Modules.Commands.DeleteModule
{
    public class DeleteModuleCommandHandler  : IRequestHandler<DeleteModuleCommand>
    {
        private readonly IModuleCommandRepository _moduleRepository;

    public DeleteModuleCommandHandler(IModuleCommandRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }

    public async Task Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
    {
        await _moduleRepository.DeleteModuleAsync(request.ModuleId);
        await _moduleRepository.SaveChangesAsync();

    }
    }
}