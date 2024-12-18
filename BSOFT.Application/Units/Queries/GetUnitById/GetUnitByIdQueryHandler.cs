using BSOFT.Application.Units.Queries.GetUnits;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Units.Queries.GetUnitById
{
    public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery,UnitVm>
    {
      private readonly IUnitRepository _unitRepository;
     private readonly IMapper _mapper;

         public GetUnitByIdQueryHandler(IUnitRepository unitRepository, IMapper mapper)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
        }
        public async Task<UnitVm> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
        {
          var unit = await _unitRepository.GetByIdAsync(request.UnitId);
          return _mapper.Map<UnitVm>(unit);
        }
    }
}