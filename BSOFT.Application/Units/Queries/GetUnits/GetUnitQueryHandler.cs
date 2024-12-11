using AutoMapper;
using BSOFT.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Units.Queries.GetUnits
{
    public class GetUnitQueryHandler : IRequestHandler<GetUnitQuery,List<UnitVm>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public GetUnitQueryHandler(IUnitRepository unitRepository , IMapper mapper)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
        }
        public async Task<List<UnitVm>> Handle(GetUnitQuery request, CancellationToken cancellationToken)
        {
            var units = await _unitRepository.GetAllUnitsAsync();
            var unitList = _mapper.Map<List<UnitVm>>(units);
            return unitList;
        }
    }
}