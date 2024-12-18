using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Application.Common.Interfaces;
using MediatR;
using System.Text;
using BSOFT.Domain.Entities;



namespace BSOFT.Application.Divisions.Queries.GetDivisionAutoComplete
{
    public class GetDivisionAutoCompleteQueryHandler : IRequestHandler<GetDivisionAutoCompleteQuery,List<DivisionAutoCompleteVm>>
    {
        private readonly IDivisionRepository _divisionRepository;
        private readonly IMapper _mapper;
         public GetDivisionAutoCompleteQueryHandler(IDivisionRepository divisionRepository, IMapper mapper)
         {
             _divisionRepository =divisionRepository;
            _mapper =mapper;
         }  
          public async Task<List<DivisionAutoCompleteVm>> Handle(GetDivisionAutoCompleteQuery request, CancellationToken cancellationToken)
          {
                
       
            var division = await _divisionRepository.GetDivision(request.SearchPattern);
            // Map to the application-specific DTO
            return division.Select(d => new DivisionAutoCompleteVm
            {
                DivId = d.DivId,
                Name = d.Name
            }).ToList();

         } 
    }
}