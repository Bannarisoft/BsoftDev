using Dapper;
using System.Data;
using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUser;
using Polly;
using Polly.Timeout;
using Serilog;

namespace UserManagement.Infrastructure.Repositories
{
     public class UserCommandRepository : IUserCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        private readonly IDbConnection _dbConnection;
        private readonly IAsyncPolicy _retryPolicy;
        private readonly IAsyncPolicy _circuitBreakerPolicy;
        private readonly IAsyncPolicy _timeoutPolicy;
        private readonly IAsyncPolicy _fallbackPolicy;
        private readonly HttpClient _httpClient;
        

		public UserCommandRepository(ApplicationDbContext applicationDbContext,IDbConnection dbConnection, IHttpClientFactory httpClientFactory)
        {
            _applicationDbContext = applicationDbContext;

            _dbConnection = dbConnection;

        
        // Create an HttpClient using IHttpClientFactory and the registered "ResilientHttpClient"
            _httpClient = httpClientFactory.CreateClient("ResilientHttpClient");
        // Define Polly policies

          // Retry policy: Retry 3 times with an exponential backoff strategy
              _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Log.Warning($"Retry {retryCount} after {timeSpan.TotalSeconds}s due to {exception.GetType().Name}: {exception.Message}");
                    });

        // Circuit Breaker policy: Break after 2 consecutive failures for 30 seconds
            _circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));

        // Timeout policy: 5 seconds timeout for the queries
             _timeoutPolicy = Policy.TimeoutAsync(5, TimeoutStrategy.Pessimistic, onTimeoutAsync: (context, timespan, task) =>
            {
                Log.Error($"Timeout after {timespan.TotalSeconds}s.");
                return Task.CompletedTask;
            });


        // Fallback policy: Return 0 or a default value in case of failure
            //   _fallbackPolicy = Policy
            //     .Handle<Exception>()
            //     .FallbackAsync(async (cancellationToken) =>
            //     {
            //         Log.Warning("Executing fallback policy due to a failure.");
            //     });
        }

          public async Task<User> CreateAsync(User user)
            {
                   var policyWrap = Policy.WrapAsync( _retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);   
                  return await policyWrap.ExecuteAsync(async () =>
                  {       
                      await _applicationDbContext.User.AddAsync(user);
                      await _applicationDbContext.SaveChangesAsync();
                      return user;
                  });

            }
        
        // public async Task<List<User>> GetAllUsersAsync()
        //  {
        //       const string query = "SELECT * FROM AppSecurity.Users";
        //       return (await _dbConnection.QueryAsync<User>(query)).ToList();
        //  }
     
        public async Task<bool> DeleteAsync(int userId,User user)
        {

            var policyWrap = Policy.WrapAsync( _retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);         
            return await policyWrap.ExecuteAsync(async () =>
            {
                var existingUser = await _applicationDbContext.User.FirstOrDefaultAsync(u => u.UserId == userId);
                if (existingUser != null)
                {
                    existingUser.IsDeleted = user.IsDeleted;
                    return await _applicationDbContext.SaveChangesAsync() > 0;
                }
                
                 return false; // No user found
            });
      
        }
        public async Task<int> UpdateAsync(int userId, User user)
        {
            var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);
            return await policyWrap.ExecuteAsync(async () =>
            {
                var existingUser = await _applicationDbContext.User
                    .Include(uc => uc.UserCompanies)
                    .Include(ur => ur.UserRoleAllocations)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                    if (existingUser != null)
                    {
                        existingUser.UserId = user.UserId;
                        existingUser.FirstName = user.FirstName;
                        existingUser.LastName = user.LastName;
                        existingUser.UserName = user.UserName;
                        existingUser.PasswordHash = user.PasswordHash;
                        existingUser.UserType = user.UserType;
                        existingUser.Mobile = user.Mobile;
                        existingUser.EmailId = user.EmailId;
                        existingUser.CompanyId = user.CompanyId;
                        existingUser.DivisionId = user.DivisionId;
                        existingUser.UnitId = user.UnitId;
                        // existingUser.UserRoleId = user.UserRoleId;
                        existingUser.IsFirstTimeUser = user.IsFirstTimeUser;
                        existingUser.IsActive = user.IsActive;

                         var updatedCompanyIds = user.UserCompanies.Select(uc => uc.CompanyId).ToList();
                         foreach (var existingCompany in existingUser.UserCompanies)
                         {
                             existingCompany.IsActive = updatedCompanyIds.Contains(existingCompany.CompanyId) ? (byte)1 : (byte)0;
                         }

                        var newCompanyIds = updatedCompanyIds
                         .Where(id => !existingUser.UserCompanies.Any(uc => uc.CompanyId == id))
                         .ToList();
                         foreach (var newCompanyId in newCompanyIds)
                         {
                             existingUser.UserCompanies.Add(new UserCompany
                             {
                                 UserId = existingUser.UserId,
                                 CompanyId = newCompanyId,
                                 IsActive = 1
                             });
                         }

                          var updatedRoleIds = user.UserRoleAllocations.Select(ur => ur.UserRoleId).ToList();
                          foreach (var existingRole in existingUser.UserRoleAllocations)
                          {
                              existingRole.IsActive = updatedRoleIds.Contains(existingRole.UserRoleId) ? (byte)1 : (byte)0;
                          }

                          var newRoleIds = updatedRoleIds
                              .Where(id => !existingUser.UserRoleAllocations.Any(ur => ur.UserRoleId == id))
                              .ToList();

                          foreach (var newRoleId in newRoleIds)
                          {
                              existingUser.UserRoleAllocations.Add(new Core.Domain.Entities.UserRoleAllocation
                              {
                                  UserId = existingUser.UserId,
                                  UserRoleId = newRoleId,
                                  IsActive = 1
                              });
                          }
                        // _applicationDbContext.UserCompanies.RemoveRange(existingUser.UserCompanies);
                        // _applicationDbContext.UserRoleAllocations.RemoveRange(existingUser.UserRoleAllocations);

                        //  existingUser.UserCompanies = user.UserCompanies.Select(uc => new UserCompany
                        //    {
                        //        UserId = existingUser.UserId,
                        //        CompanyId = uc.CompanyId
                        //    }).ToList();

                        //    existingUser.UserRoleAllocations = user.UserRoleAllocations.Select(ur => new Core.Domain.Entities.UserRoleAllocation
                        //    {
                        //        UserId = existingUser.UserId,
                        //        UserRoleId = ur.UserRoleId
                        //    }).ToList();

                        _applicationDbContext.User.Update(existingUser);
                    return await _applicationDbContext.SaveChangesAsync();
                 }
                 return 0; // No user found
            });
        }

    }
}
