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
        private readonly IMapper _imapper;
        public DeleteDivisionCommandHandler(IDivisionRepository divisionRepository, IMapper imapper)
        {
            _divisionRepository = divisionRepository;
            _imapper = imapper;
        }
         public async Task<int> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            var division  = _imapper.Map<Division>(request);
            var divisionresult = await _divisionRepository.DeleteAsync(request.Id, division);
          
            return divisionresult;
        }
    }
}