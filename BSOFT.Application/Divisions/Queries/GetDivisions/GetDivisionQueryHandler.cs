using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Domain.Interfaces;
using MediatR;
using System.Text;

namespace BSOFT.Application.Divisions.Queries.GetDivisions
{
    public class GetDivisionQueryHandler : IRequestHandler<GetDivisionQuery,List<DivisionVm>>
    {
        private readonly IDivisionRepository _divisionRepository;
        private readonly IMapper _mapper;
        public GetDivisionQueryHandler(IDivisionRepository divisionRepository, IMapper mapper)
        {
            _divisionRepository =divisionRepository;
            _mapper =mapper;
        }
        public async Task<List<DivisionVm>> Handle(GetDivisionQuery requst, CancellationToken cancellationToken){

            var divisions = await _divisionRepository.GetAllDivisionAsync();
            
            var divisionlist = _mapper.Map<List<DivisionVm>>(divisions);

            return divisionlist;
        }
    }
}