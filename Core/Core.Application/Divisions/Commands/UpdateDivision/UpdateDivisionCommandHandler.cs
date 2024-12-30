using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Divisions.Commands.UpdateDivision
{
    public class UpdateDivisionCommandHandler : IRequestHandler<UpdateDivisionCommand, int>
    {
        private readonly IDivisionRepository _divisionRepository;
        public UpdateDivisionCommandHandler(IDivisionRepository divisionRepository)
        {
            _divisionRepository =divisionRepository;
        }
          public async Task<int> Handle(UpdateDivisionCommand request, CancellationToken cancellationToken)
        {
            var Updatedivision = new Division()
            {
                DivId = request.DivId,
                ShortName = request.ShortName,
                Name = request.Name,
                CompanyId = request.CompanyId 
            };

            return await _divisionRepository.UpdateAsync(request.DivId, Updatedivision);
        }
        
    }
}