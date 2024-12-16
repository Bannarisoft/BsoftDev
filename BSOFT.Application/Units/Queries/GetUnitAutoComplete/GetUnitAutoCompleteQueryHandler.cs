using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Domain.Interfaces;
using MediatR;
using System.Text;


namespace BSOFT.Application.Units.Queries.GetUnitAutoComplete
{
    public class GetUnitAutoCompleteQueryHandler : IRequestHandler<GetUnitAutoCompleteQuery,List<UnitAutoCompleteVm>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;
         public GetUnitAutoCompleteQueryHandler(IUnitRepository unitRepository, IMapper mapper)
         {
             _unitRepository =unitRepository;
            _mapper =mapper;
         }  
          public async Task<List<UnitAutoCompleteVm>> Handle(GetUnitAutoCompleteQuery request, CancellationToken cancellationToken)
          {
                
       
            var unit = await _unitRepository.GetUnit(request.SearchPattern);
            // Map to the application-specific DTO
            return unit.Select(r => new UnitAutoCompleteVm
            {
                UnitId = r.UnitId,
                UnitName = r.UnitName
            }).ToList();

         } 
    }
}