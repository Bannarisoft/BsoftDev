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
using Dapper;



namespace Core.Application.Divisions.Queries.GetDivisionAutoComplete
{
    public class GetDivisionAutoCompleteQueryHandler : IRequestHandler<GetDivisionAutoCompleteQuery,List<DivisionAutoCompleteDTO>>
    {
        // private readonly IDivisionRepository _divisionRepository;
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _mapper;
         public GetDivisionAutoCompleteQueryHandler(IDbConnection dbConnection, IMapper mapper)
         {
            _dbConnection = dbConnection;
            _mapper =mapper;
         }  
          public async Task<List<DivisionAutoCompleteDTO>> Handle(GetDivisionAutoCompleteQuery request, CancellationToken cancellationToken)
          {
                var searchPattern = "%" + request.SearchPattern + "%";

                 const string query = @"
                 SELECT 
                Id, 
                Name
            FROM AppData.Division where Name like @SearchPattern";

            var division = await _dbConnection.QueryAsync<DivisionAutoCompleteDTO>(query, new { SearchPattern = searchPattern });
            // Map to the application-specific DTO
            return division.AsList();
         } 
    }
}