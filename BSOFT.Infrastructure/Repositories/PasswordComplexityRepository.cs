using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;


namespace BSOFT.Infrastructure.Repositories
{
    public class PasswordComplexityRepository   :IPasswordComplexityRepository
    {

           private readonly ApplicationDbContext _applicationDbContext;
        

        public PasswordComplexityRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }


         public async Task<List<PasswordComplexityRule>>GetPasswordComplexityAsync()
         {
        
            return await _applicationDbContext.PasswordComplexityRule.ToListAsync();
        
         }

        public async Task<PasswordComplexityRule> CreateAsync(PasswordComplexityRule passwordComplexityRule)
        {
                await _applicationDbContext.PasswordComplexityRule.AddAsync(passwordComplexityRule);
                await _applicationDbContext.SaveChangesAsync();
                return passwordComplexityRule;
        }
             public async Task<int>UpdateAsync(int id, PasswordComplexityRule passwordComplexityRule)
        {
            var existingpwdcomrule  = await _applicationDbContext.PasswordComplexityRule.FirstOrDefaultAsync(p => p.Id == id);
            if (existingpwdcomrule  != null)
            {
                existingpwdcomrule.PwdComplexityRule = passwordComplexityRule.PwdComplexityRule;     
                existingpwdcomrule.IsActive = passwordComplexityRule.IsActive;                                

                _applicationDbContext.PasswordComplexityRule.Update(existingpwdcomrule);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
         }
           public async Task<int> DeleteAsync(int id ,PasswordComplexityRule pwdcomplexityrule )
           {                        
            var PwdcomplexityruleToDelete = await _applicationDbContext.PasswordComplexityRule.FirstOrDefaultAsync(u => u.Id == id);
            
            if (PwdcomplexityruleToDelete != null)
            {               
                PwdcomplexityruleToDelete.IsActive = pwdcomplexityrule.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; 
           }
    }
}