using BSOFT.Application.Divisions.Queries.GetDivisions;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Divisions.Commands.CreateDivision
{
    public class CreateDivisionCommandHandler : IRequestHandler<CreateDivisionCommand, DivisionVm>
    {
         private readonly IDivisionRepository _divisionRepository;
        private readonly IMapper _mapper;

        public CreateDivisionCommandHandler(IDivisionRepository divisionRepository, IMapper mapper)
        {
            _divisionRepository = divisionRepository;
            _mapper = mapper;
        }

        public async Task<DivisionVm> Handle(CreateDivisionCommand request, CancellationToken cancellationToken)
        {
            var division = new Division
            {
                ShortName = request.ShortName,
                Name = request.Name,
                CompanyId = request.CompanyId,
                IsActive = request.IsActive          
            };

            var result = await _divisionRepository.CreateAsync(division);
            return _mapper.Map<DivisionVm>(result);
        }
    }
}