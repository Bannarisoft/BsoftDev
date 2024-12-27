using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Application.Divisions.Queries.GetDivisions;

namespace Core.Application.Divisions.Queries.GetDivisionById
{
    public class GetDivisionByIdQueryHandler : IRequestHandler<GetDivisionByIdQuery,DivisionVm>
    {
        private readonly IDivisionRepository _divisionRepository;
        private readonly IMapper _mapper;

         public GetDivisionByIdQueryHandler(IDivisionRepository divisionRepository, IMapper mapper)
        {
            _divisionRepository =divisionRepository;
            _mapper =mapper;
        } 
        public async Task<DivisionVm> Handle(GetDivisionByIdQuery request, CancellationToken cancellationToken)
        {
           

          var division = await _divisionRepository.GetByIdAsync(request.DivId);
          return _mapper.Map<DivisionVm>(division);
        }
    }
}