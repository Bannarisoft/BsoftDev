using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Core.Application.Common.Interfaces.ICompany;
using Core.Application.Common.Interfaces;
using System.Data;
using Dapper;
using Core.Domain.Entities;

namespace BSOFT.Infrastructure.Repositories.Companies
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
                GstNumber,
                TIN,
                TAN,
                CSTNo,
                YearOfEstablishment,
                Website,
                Logo,
                EntityId, 
                IsActive,
                AddressLine1,
                AddressLine2,
                PinCode,
                CountryId,
                StateId,
                CityId,
                A.Phone AS AddressPhone, 
                Name,
                Designation,
                Email,
                B.Phone AS ContactPhone,
                Remark
            FROM AppData.Company C
            LEFT JOIN AppData.CompanyAddress A ON A.CompanyId = C.Id
            LEFT JOIN AppData.CompanyContact B ON B.CompanyId = C.Id";
            return (await _dbConnection.QueryAsync<Company>(query)).ToList();
        }
         public async Task<Company> GetByIdAsync(int id)
        {            
            const string query = @"  SELECT 
                C.Id, 
                C.CompanyName, 
                C.LegalName,
                GstNumber,
                TIN,
                TAN,
                CSTNo,
                YearOfEstablishment,
                Website,
                Logo,
                EntityId, 
                IsActive,
                AddressLine1,
                AddressLine2,
                PinCode,
                CountryId,
                StateId,
                CityId,
                A.Phone AS AddressPhone, 
                Name,
                Designation,
                Email,
                B.Phone AS ContactPhone,
                Remark
            FROM AppData.Company C
            LEFT JOIN AppData.CompanyAddress A ON A.CompanyId = C.Id
            LEFT JOIN AppData.CompanyContact B ON B.CompanyId = C.Id WHERE C.Id = @CompanyId";
            return await _dbConnection.QueryFirstOrDefaultAsync<Company>(query, new { id });
        }
        
         public async Task<List<Company>>  GetCompany(string searchPattern = null)
        {
                if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("DivisionName cannot be null or empty.", nameof(searchPattern));
            }

            const string query = @"
                SELECT 
                Id, 
                CompanyName
            FROM AppData.Company where CompanyName like @SearchPattern";
                
            // Update the object to use SearchPattern instead of Name
            var divisions = await _dbConnection.QueryAsync<Company>(query, new { SearchPattern = $"%{searchPattern}%" });
            return divisions.ToList();
        }
    }
}