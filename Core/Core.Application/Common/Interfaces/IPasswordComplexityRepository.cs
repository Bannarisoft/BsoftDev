using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces
{
    public interface IPasswordComplexityRepository
    {
        Task<List<Core.Domain.Entities.PasswordComplexityRule>> GetPasswordComplexityAsync();
        Task<Core.Domain.Entities.PasswordComplexityRule> CreateAsync(Core.Domain.Entities.PasswordComplexityRule passwordComplexityRule);
        Task<int> UpdateAsync(int id, Core.Domain.Entities.PasswordComplexityRule passwordComplexityRule);
        Task<int> DeleteAsync(int id, Core.Domain.Entities.PasswordComplexityRule passwordComplexityRule);
              

       
       
    }
}