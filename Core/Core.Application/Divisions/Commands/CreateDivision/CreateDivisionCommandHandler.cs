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
                IsActive = 1         
            };

            var result = await _divisionRepository.CreateAsync(division);
            return _mapper.Map<DivisionVm>(result);
        }
    }
}