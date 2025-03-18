using Dapper;
using System.Data;
using UserManagement.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUser;

using Polly;
using Polly.Timeout;
using Serilog;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Core.Domain.Common;

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
        public async Task<(List<User>,int)> GetAllUsersAsync(int PageNumber, int PageSize, string? SearchTerm)
        {

                     var query = $$"""

                     DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM AppSecurity.Users 
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (FirstName LIKE @Search OR LastName LIKE @Search OR UserName LIKE @Search)")}};

                SELECT DISTINCT ur.Id,
                                ur.UserId,
                                ur.FirstName,
                                ur.LastName,
                                ur.UserName,
                                ur.IsActive,
                                ur.PasswordHash,
                                ur.UserType,
                                ur.Mobile,
                                ur.EmailId,
                                ur.IsFirstTimeUser,
                                ur.IsDeleted,UG.Id AS UserGroupId
                FROM AppSecurity.Users ur
                left join AppSecurity.UserGroup UG on UG.Id=ur.UserGroupId and UG.IsActive=1
                WHERE ur.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ur.FirstName LIKE @Search OR ur.LastName LIKE @Search OR ur.UserName LIKE @Search)")}}
                ORDER BY ur.UserId desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            """;

             var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };
                    var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);

                    return await policyWrap.ExecuteAsync(async () =>
                    {
                          var user = await _dbConnection.QueryMultipleAsync(query, parameters);
                          var userlist = (await user.ReadAsync<User>()).ToList();
                          int totalCount = (await user.ReadFirstAsync<int>());
                          
                          return  (userlist, totalCount);
                        

                        
                    });
        }

     public async Task<User?> GetByIdAsync(int userId)
{
    const string query = @"
        SELECT ur.Id,
               ur.UserId,
               ur.FirstName,
               ur.LastName,
               ur.UserName,
               ur.IsActive,
               ur.PasswordHash,
               ur.UserType,
               ur.Mobile,
               ur.EmailId,
               ur.IsFirstTimeUser,
               ur.IsDeleted,
               ura.UserRoleId,
               uc.CompanyId,
               uu.UnitId,
               ud.DivisionId,
               UG.Id AS UserGroupId
        FROM AppSecurity.Users ur
        LEFT JOIN AppSecurity.UserRoleAllocation ura ON ur.UserId = ura.UserId AND ura.IsActive = 1
        LEFT JOIN AppSecurity.UserCompany uc ON uc.UserId = ur.UserId AND uc.IsActive = 1
        LEFT JOIN AppSecurity.UserUnit uu ON uu.UserId = ur.UserId AND uu.IsActive = 1
        LEFT JOIN AppSecurity.UserDivision ud ON ud.UserId = ur.UserId AND ud.IsActive = 1
        LEFT JOIN AppSecurity.UserGroup UG ON UG.Id = ur.UserGroupId AND UG.IsActive = 1
        WHERE ur.IsDeleted = 0 AND ur.UserId = @UserId";

    var userDictionary = new Dictionary<int, User>();

    var userResponse = await _dbConnection.QueryAsync<User,Core.Domain.Entities.UserRoleAllocation, UserCompany, UserUnit, UserDivision, int?, User>(
        query,
        (user, userRole, userCompany, userUnit, userDivision, userGroupId) =>
        {
            if (!userDictionary.TryGetValue(user.UserId, out var existingUser))
            {
                existingUser = user;
                existingUser.UserRoleAllocations = new List<Core.Domain.Entities.UserRoleAllocation>();
                existingUser.UserCompanies = new List<UserCompany>();
                existingUser.UserUnits = new List<UserUnit>();
                existingUser.UserDivisions = new List<UserDivision>();
                userDictionary[user.UserId] = existingUser;
            }

            // ✅ Append user roles
            if (userRole != null && !existingUser.UserRoleAllocations.Any(ur => ur.UserRoleId == userRole.UserRoleId))
            {
                existingUser.UserRoleAllocations.Add(userRole);
            }

            // ✅ Append user companies
            if (userCompany != null && !existingUser.UserCompanies.Any(uc => uc.CompanyId == userCompany.CompanyId))
            {
                existingUser.UserCompanies.Add(userCompany);
            }

            // ✅ Append user units
            if (userUnit != null && !existingUser.UserUnits.Any(uu => uu.UnitId == userUnit.UnitId))
            {
                existingUser.UserUnits.Add(userUnit);
            }

            // ✅ Append user divisions
            if (userDivision != null && !existingUser.UserDivisions.Any(ud => ud.DivisionId == userDivision.DivisionId))
            {
                existingUser.UserDivisions.Add(userDivision);
            }

            // ✅ Assign UserGroupId
            if (userGroupId.HasValue)
            {
                existingUser.UserGroupId = userGroupId.Value;
            }

            return existingUser;
        },
        new { userId },
        splitOn: "UserRoleId,CompanyId,UnitId,DivisionId,UserGroupId" // ✅ Added UserGroupId here
    );

    return userResponse.FirstOrDefault();
}



        public async Task<User?> GetByUsernameAsync(string? username, int? id = null)
        {
           
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
            
               var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);
            
               return await policyWrap.ExecuteAsync(async () =>
               {
                   return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, parameters);
               });
          
        }
        public async Task<List<User>> GetUser(string searchPattern)
        {
             const string query = @"
                SELECT UserId, UserName 
                FROM AppSecurity.Users
                WHERE IsDeleted = 0 AND UserName LIKE @SearchPattern";
                
            
            var users = await _dbConnection.QueryAsync<User>(query, new { SearchPattern = $"%{searchPattern}%" });
            
            
               var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);
            
               return await policyWrap.ExecuteAsync(async () =>
               {
                   return users.ToList();
               });
          
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
          public async Task<bool> AlreadyExistsAsync(string username, int? id = null)
          {
              var query = "SELECT COUNT(1) FROM [AppSecurity].[Users] WHERE UserName = @UserName AND IsDeleted = 0";
                var parameters = new DynamicParameters(new { Username = username });

             if (id is not null)
             {
                 query += " AND UserId != @Id";
                 parameters.Add("Id", id);
             }
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
          }
          public async Task<User?> GetByUserByUnit(int UserId,int UnitId)
          {
            const string query = @"
                SELECT U.UserId, U.UserName,U.Mobile,U.EmailId
                FROM AppSecurity.Users U
                Inner join [AppSecurity].[UserUnit] UU on UU.UserId = U.UserId
                Inner join [AppData].[Unit] U1 on U1.Id = UU.UnitId
                Inner join [AppData].[Division] D on D.Id = U1.DivisionId
                Inner join [AppData].[Company] C on C.Id = U1.CompanyId
                WHERE U.IsDeleted = 0 AND U.UserId = @UserId and UU.UnitId = @UnitId";
                
            
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new
             {
                 UserId,
                 UnitId
             });
            
            
          }
        
    }
}
