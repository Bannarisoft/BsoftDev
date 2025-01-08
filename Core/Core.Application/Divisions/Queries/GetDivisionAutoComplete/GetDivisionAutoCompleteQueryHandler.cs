using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Domain.Entities;
using Core.Application.Divisions.Queries.GetDivisions;
using System.Data;
using Core.Application.Common.Interfaces.IDivision;



namespace Core.Application.Divisions.Queries.GetDivisionAutoComplete
{
    public class GetDivisionAutoCompleteQueryHandler : IRequestHandler<GetDivisionAutoCompleteQuery,List<DivisionAutoCompleteDTO>>
    {
        private readonly IDivisionQueryRepository _divisionRepository;
        private readonly IMapper _mapper;
         public GetDivisionAutoCompleteQueryHandler(IDivisionQueryRepository divisionRepository, IMapper mapper)
         {
            _divisionRepository =divisionRepository;
            _mapper =mapper;
         }  
          public async Task<List<DivisionAutoCompleteDTO>> Handle(GetDivisionAutoCompleteQuery request, CancellationToken cancellationToken)
          {
             /*    var searchPattern = "%" + request.SearchPattern + "%";

                 const string query = @"
                 SELECT 
                Id, 
                Name
            FROM AppData.Division where Name like @SearchPattern";

            var division = await _dbConnection.QueryAsync<DivisionAutoCompleteDTO>(query, new { SearchPattern = searchPattern });
            // Map to the application-specific DTO
            return division.AsList(); */
            var result = await _divisionRepository.GetDivision(request.SearchPattern);
            //return _mapper.Map<List<DivisionDTO>>(result);
            return _mapper.Map<List<DivisionAutoCompleteDTO>>(result);            
         } 
    }
}