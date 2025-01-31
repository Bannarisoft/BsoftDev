using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Enums.Common;

namespace BSOFT.Infrastructure.Repositories.State
{    
    public class StateCommandRepository : IStateCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;        

        public StateCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;            
        }
        public async Task<States> CreateAsync(States states)
        {
            await _applicationDbContext.States.AddAsync(states);
            await _applicationDbContext.SaveChangesAsync();
            return states;
        }

        public async Task<int> DeleteAsync(int id, States states)
        {
             var StateToDelete = await _applicationDbContext.States.FirstOrDefaultAsync(u => u.Id == id);
            if (StateToDelete != null)
            {
                StateToDelete.IsActive = states.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }  

        public async Task<int> UpdateAsync(int id, States state)
        
        {
            var existingState = await _applicationDbContext.States.FirstOrDefaultAsync(u => u.Id == id);             
    
            if (existingState != null)
            {
                existingState.StateName = state.StateName;
                existingState.StateCode = state.StateCode;                
                existingState.CountryId = state.CountryId;
                existingState.IsActive = state.IsActive;                
                _applicationDbContext.States.Update(existingState);
                return await _applicationDbContext.SaveChangesAsync();
            }
           return 0; // No user found
        }
        public async Task<bool> CountryExistsAsync(int countryId)
        {
        //return await _applicationDbContext.States.AnyAsync(c => c.Id == stateId);
            return await _applicationDbContext.Countries.AnyAsync(s => s.Id == countryId && s.IsDeleted == Enums.IsDelete.Deleted && s.IsActive == Enums.Status.Active);
        }
        public async Task<bool> GetStateByCodeAsync(string stateName,string stateCode, int countryId)
        {
                var state = await _applicationDbContext.States
                    .FirstOrDefaultAsync(s => s.StateCode == stateCode 
                                  && s.StateName == stateName && s.CountryId == countryId);

                if (state != null)
                {                    
                    if ((byte)state.IsActive == (byte)Enums.Status.Inactive)
                    {                        
                        return false;  // You can adjust this to return a message indicating it's inactive
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
        }
    }
}
