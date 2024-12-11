using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;

namespace BSOFT.Application.Divisions.Commands.UpdateDivision
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
                CompanyId = request.CompanyId,
                ModifiedBy = request.ModifiedBy, 
                ModifiedAt = request.ModifiedAt,
                ModifiedByName = request.ModifiedByName,
                ModifiedIP = request.ModifiedIP  
            };

            return await _divisionRepository.UpdateAsync(request.DivId, Updatedivision);
        }
        
    }
}