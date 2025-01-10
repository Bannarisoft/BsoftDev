using AutoMapper;
using Core.Application.Common.Interfaces.IUnit;
using Core.Application.Units.Queries.GetUnits;
using MediatR;
using System.Data;

namespace Core.Application.Units.Queries.GetUnitById
{
    //public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery,UnitDto>
    public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery,List<UnitDto>>
    {
         private readonly IUnitQueryRepository _unitRepository;        
        private readonly IMapper _mapper;

        public GetUnitByIdQueryHandler(IUnitQueryRepository unitRepository, IMapper mapper)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
        }

         public async Task<List<UnitDto>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitRepository.GetByIdAsync(request.Id);            
            var unitList = _mapper.Map<List<UnitDto>>(result);
            return unitList;
        }
    }
}   