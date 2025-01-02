using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;

namespace Core.Application.Divisions.Commands.DeleteDivision
{
    public class DeleteDivisionCommandHandler : IRequestHandler<DeleteDivisionCommand, int>
    {
        private readonly IDivisionRepository _divisionRepository;
        public DeleteDivisionCommandHandler(IDivisionRepository divisionRepository)
        {
            _divisionRepository = divisionRepository;
        }
         public async Task<int> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            var Updatedivision = new Division()
            {
                DivId = request.DivId,
                IsActive = request.IsActive
            };
            return await _divisionRepository.DeleteAsync(request.DivId,Updatedivision);
        }
    }
}