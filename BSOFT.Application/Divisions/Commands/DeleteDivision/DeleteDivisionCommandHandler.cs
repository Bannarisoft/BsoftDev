using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using AutoMapper;

namespace BSOFT.Application.Divisions.Commands.DeleteDivision
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