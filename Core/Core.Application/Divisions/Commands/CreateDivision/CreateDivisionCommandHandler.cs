using Core.Application.Divisions.Queries.GetDivisions;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Divisions.Commands.CreateDivision
{
    public class CreateDivisionCommandHandler : IRequestHandler<CreateDivisionCommand, int>
    {
         private readonly IDivisionRepository _divisionRepository;
        private readonly IMapper _imapper;

        public CreateDivisionCommandHandler(IDivisionRepository divisionRepository, IMapper imapper)
        {
            _divisionRepository = divisionRepository;
            _imapper = imapper;
        }

        public async Task<int> Handle(CreateDivisionCommand request, CancellationToken cancellationToken)
        {
            var division  = _imapper.Map<Division>(request);

            var divisionresult = await _divisionRepository.CreateAsync(division);
            return divisionresult.Id;
        }
    }
}