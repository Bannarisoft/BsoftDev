using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Application.Companies.Queries.GetCompanies;
using System.Data;
using Dapper;



namespace Core.Application.Companies.Queries.GetCompanyAutoComplete
{
    public class GetCompanyAutoCompleteQueryHandler : IRequestHandler<GetCompanyAutoCompleteQuery,List<CompanyAutoCompleteDTO>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _imapper;
         public GetCompanyAutoCompleteQueryHandler(IDbConnection dbConnection, IMapper imapper)
         {
             _dbConnection = dbConnection;
            _imapper =imapper;
         }  
          public async Task<List<CompanyAutoCompleteDTO>> Handle(GetCompanyAutoCompleteQuery request, CancellationToken cancellationToken)
          {
            var searchPattern = "%" + request.SearchPattern + "%";

                 const string query = @"
                 SELECT 
                Id, 
                CompanyName
            FROM AppData.Company where CompanyName like @SearchPattern";

            var company = await _dbConnection.QueryAsync<CompanyAutoCompleteDTO>(query, new { SearchPattern = searchPattern });
            // Map to the application-specific DTO
            // return company.Select(c => new CompanyAutoCompleteDTO
            // {
            //     CompanyId = c.Id,
            //     CompanyName = c.CompanyName
            // }).ToList();
            return company.AsList();

         } 
    }
}