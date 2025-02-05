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

         public async Task<List<Company>> GetAllCompaniesAsync()
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
                    A.Phone AS AddressPhone, 
                    B.Name,
                    B.Designation,
                    B.Email,
                    B.Phone AS ContactPhone,
                    B.Remark AS Remarks 
                FROM AppData.Company C
                LEFT JOIN AppData.CompanyAddress A ON A.CompanyId = C.Id
                LEFT JOIN AppData.CompanyContact B ON B.CompanyId = C.Id
                WHERE C.IsDeleted = 0";
            
            var companyDictionary = new Dictionary<int, Company>();
            
            var companies = await _dbConnection.QueryAsync<Company, CompanyAddress, CompanyContact, Company>(
                query,
                (company, address, contact) =>
                {
                    if (!companyDictionary.TryGetValue(company.Id, out var existingCompany))
                    {
                        existingCompany = company;
                        existingCompany.CompanyAddress = address;
                        existingCompany.CompanyContact = contact;
                        companyDictionary.Add(existingCompany.Id, existingCompany);
                    }
                    else
                    {
                        existingCompany.CompanyAddress = address;
                        existingCompany.CompanyContact = contact;
                    }
            
                    return existingCompany;
                },
                splitOn: "AddressLine1,Name" 
            );
            
            return companies.Distinct().ToList();

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
    }
}