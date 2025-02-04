using Dapper;
using System.Data;
using UserManagement.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUser;

using Polly;
using Polly.Timeout;
using Serilog;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace UserManagement.Infrastructure.Repositories.Users
{
    public class UserQueryRepository : IUserQueryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IDbConnection _dbConnection;
        private readonly IAsyncPolicy _retryPolicy;
        private readonly IAsyncPolicy _circuitBreakerPolicy;
        private readonly IAsyncPolicy _timeoutPolicy;
        private readonly IAsyncPolicy _fallbackPolicy;

        public UserQueryRepository(ApplicationDbContext applicationDbContext,IDbConnection dbConnection)
        {
            _applicationDbContext = applicationDbContext;

            _dbConnection = dbConnection;
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

        // Fallback policy: Return an empty list in case of an error
            // _fallbackPolicy = Policy<List<User>>.Handle<Exception>()
            //     .FallbackAsync(new List<User>());
        }
        public async Task<List<User>> GetAllUsersAsync()
        {

             const string query = @"SELECT ur.Id,
             ur.UserId,
                         ur.DivisionId,
                         ur.FirstName,
                         ur.LastName,
                         ur.UserName,
                         ur.IsActive,
                         ur.PasswordHash,
                         ur.UserType,
                         ur.Mobile,
                         ur.EmailId,
                         ur.UnitId,
                         ur.UserId,
                         ur.IsFirstTimeUser,
                         ur.IsDeleted,
                         ura.UserRoleId,
                         uc.CompanyId 
                         FROM AppSecurity.Users ur
                     Left JOIN AppSecurity.UserRoleAllocation ura ON   ur.UserId = ura.UserId and ura.IsActive = 1
                     Left JOIN AppSecurity.UserCompany uc ON uc.UserId = ur.UserId and uc.IsActive = 1
                     WHERE  ur.IsDeleted = 0";

              var userDictionary = new Dictionary<int, User>();

              var users = await _dbConnection.QueryAsync<User, Core.Domain.Entities.UserRoleAllocation, UserCompany, User>(
             query,
             (user, userRole, userCompany) =>
             {
                 if (!userDictionary.TryGetValue(user.UserId, out var existingUser))
                 {
                     existingUser = user;
                     existingUser.UserRoleAllocations = new List<Core.Domain.Entities.UserRoleAllocation> { userRole };
                     existingUser.UserCompanies = new List<UserCompany> { userCompany };
                     userDictionary.Add(existingUser.UserId, existingUser);
                 }
                 else
                 {
                     existingUser.UserRoleAllocations.Add(userRole);
                     existingUser.UserCompanies.Add(userCompany);
                 }

                 return existingUser;
             },
             splitOn: "UserRoleId,CompanyId"  
             );
            var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);
            return await policyWrap.ExecuteAsync(async () =>
            {
                 return users.Distinct().ToList();
            });
        }

        public async Task<User?> GetByIdAsync(int userId)
        {

             const string query = @"
             SELECT ur.Id,
                    ur.UserId,
                    ur.DivisionId,
                    ur.FirstName,
                    ur.LastName,
                    ur.UserName,
                    ur.IsActive,
                    ur.PasswordHash,
                    ur.UserType,
                    ur.Mobile,
                    ur.EmailId,
                    ur.UnitId,
                    ur.UserId,
                    ur.IsFirstTimeUser,
                    ur.IsDeleted,
                    ura.UserRoleId,
                    uc.CompanyId 
                    FROM AppSecurity.Users ur
                Left JOIN AppSecurity.UserRoleAllocation ura ON   ur.UserId = ura.UserId and ura.IsActive = 1
                Left JOIN AppSecurity.UserCompany uc ON uc.UserId = ur.UserId and uc.IsActive = 1
                WHERE  ur.IsDeleted = 0 and ur.UserId = @UserId";
          var userResponse = await _dbConnection.QueryAsync<User, Core.Domain.Entities.UserRoleAllocation, UserCompany, User>(query, 
          (user, userRole, userCompany) =>
          {
              user.UserRoleAllocations = new List<Core.Domain.Entities.UserRoleAllocation> { userRole };
              user.UserCompanies = new List<UserCompany> { userCompany };
              return user;
              }, 
          new { userId },
          splitOn: "UserRoleId,CompanyId");

            var policyWrap = Policy.WrapAsync( _retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);
            return await policyWrap.ExecuteAsync(async () =>
                        {
                        return userResponse.FirstOrDefault();
            });
            // const string query = "SELECT * FROM AppSecurity.Users WHERE UserId = @UserId";
            // return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { userId });
        }

        public async Task<User?> GetByUsernameAsync(string username, int? id = null)
        {
            
             if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }


             var query = """
                 SELECT * FROM AppSecurity.Users 
                 WHERE UserName = @Username AND IsDeleted = 0
                 """;

             var parameters = new DynamicParameters(new { Username = username });

             if (id is not null)
             {
                 query += " AND UserId != @Id";
                 parameters.Add("Id", id);
             }

            
        

            var policyWrap = Policy.WrapAsync( _retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);
            return await policyWrap.ExecuteAsync(async () =>
            {
                // Execute the Dapper query with Polly policies
                return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, parameters);
            });
            // if (string.IsNullOrWhiteSpace(username))
            // {
            //     throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            // }

            // const string query = "SELECT * FROM AppSecurity.Users WHERE UserName = @Username";
            // return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
        }
        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
                const string query = @"
                SELECT ur.RoleName
                FROM AppSecurity.UserRole ur
                INNER JOIN AppSecurity.UserRoleAllocation ura ON   ur.Id = ura.UserRoleId
                INNER JOIN AppSecurity.Users u ON u.UserId = ura.UserId
                WHERE u.UserId = @UserId and u.IsDeleted = 0";
                // const string query = @"
                // SELECT 'Admin' as RoleName FROM AppSecurity.Users u 
                // WHERE u.UserId = @UserId";

                
                var policyWrap = Policy.WrapAsync( _retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);
                return await policyWrap.ExecuteAsync(async () =>
                {
                return (await _dbConnection.QueryAsync<string>(query, new { UserId = userId })).ToList();
                });

        }
        
    }
}
