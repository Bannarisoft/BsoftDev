using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Core.Application.Common.Interfaces.ICompany;
using Core.Application.Common.Interfaces;
using System.Data;
using Dapper;
using Core.Domain.Entities;

namespace UserManagement.Infrastructure.Repositories.Companies
{
    public class CompanyQueryRepository : ICompanyQueryRepository
    {
        private readonly IDbConnection _dbConnection;        
         public CompanyQueryRepository(IDbConnection dbConnection)
        {
         _dbConnection = dbConnection;
        }

         public async Task<(List<Company>,int)> GetAllCompaniesAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
            DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM AppData.Company C
              WHERE C.IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (C.CompanyName LIKE @Search OR C.LegalName LIKE @Search)")}};

                SELECT 
            C.Id, 
            C.CompanyName, 
            C.LegalName,
            C.GstNumber,
            C.TIN,
            C.TAN,
            C.CSTNo,
            C.YearOfEstablishment,
            C.Website,
            C.Logo,
            C.EntityId, 
            C.IsActive
             FROM AppData.Company C
              WHERE C.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (C.CompanyName LIKE @Search OR C.LegalName LIKE @Search)")}}
              ORDER BY C.Id DESC
              OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

             SELECT @TotalCount AS TotalCount;
            """;
            
            
            var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };
              var company = await _dbConnection.QueryMultipleAsync(query, parameters);
             var companies = (await company.ReadAsync<Company>()).ToList();
             int totalCount = (await company.ReadFirstAsync<int>());
            return (companies, totalCount);

        }
         public async Task<Company?> GetByCompanynameAsync(string name, int? id = null)
        {

             var query = """
                 SELECT * FROM AppData.Company 
                 WHERE CompanyName = @Name AND IsDeleted = 0
                 """;

             var parameters = new DynamicParameters(new { Name = name });

             if (id is not null)
             {
                 query += " AND Id != @Id";
                 parameters.Add("Id", id);
             }

            return await _dbConnection.QueryFirstOrDefaultAsync<Company>(query, parameters);
        }
     
         public async Task<Company> GetByIdAsync(int id)
        {            
           const string query = @"
             SELECT 
                C.Id, 
            C.CompanyName, 
            C.LegalName,
            C.GstNumber,
            C.TIN,
            C.TAN,
            C.CSTNo,
            C.YearOfEstablishment,
            C.Website,
            C.Logo,
            C.EntityId, 
            C.IsActive,
            A.AddressLine1,
            A.AddressLine2,
            A.PinCode,
            A.CountryId,
            A.StateId,
            A.CityId,
            A.Phone,
            A.AlternatePhone,
            B.Name,
                 B.Designation,
                 B.Email,
                 B.Phone ,
                 B.Remark As Remarks 
             FROM AppData.Company C
             LEFT JOIN AppData.CompanyAddress A ON A.CompanyId = C.Id
             LEFT JOIN AppData.CompanyContact B ON B.CompanyId = C.Id
             WHERE C.Id = @id AND C.IsDeleted = 0";
    var companyResponse = await _dbConnection.QueryAsync<Company,CompanyAddress,CompanyContact,Company>(query, 
    (company,companyAddress,companyContact) =>
    {
        company.CompanyAddress = companyAddress;
        company.CompanyContact = companyContact;
        return company;
        }, 
    new { id },
    splitOn: "AddressLine1,Name");

             return companyResponse.FirstOrDefault();

        }
        
         public async Task<List<Company>>  GetCompany(string searchPattern = null)
        {
             

            const string query = @"
                SELECT 
                Id, 
                CompanyName
            FROM AppData.Company where IsDeleted = 0 and CompanyName like @SearchPattern";
                
            
            var result = await _dbConnection.QueryAsync<Company>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }
        public async Task<bool> CompanyExistsAsync(string companyName)
          {
              var sql = "SELECT COUNT(1) FROM AppData.Company WHERE CompanyName = @CompanyName";
                var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { CompanyName = companyName });
                return count > 0;
          }
            public async Task<bool>SoftDeleteValidation(int Id)
            {
                                const string query = @"
                           SELECT 1 
                           FROM [AppData].[CompanySetting] 
                           WHERE CompanyId = @Id AND IsDeleted = 0;
                    
                           SELECT 1 
                           FROM [AppData].[Division]
                           WHERE CompanyId = @Id AND IsDeleted = 0;
                           
                           SELECT 1 
                           FROM [AppData].[Unit]
                           WHERE CompanyId = @Id AND IsDeleted = 0;";
                    
                       using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = Id });
                    
                       var companySettingExists = await multi.ReadFirstOrDefaultAsync<int?>();  
                       var divisionExists = await multi.ReadFirstOrDefaultAsync<int?>();
                       var unitExists = await multi.ReadFirstOrDefaultAsync<int?>();
                    
                       return companySettingExists.HasValue || divisionExists.HasValue || unitExists.HasValue;
            }
        
    }
}