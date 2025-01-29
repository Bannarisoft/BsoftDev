using Dapper;
using System.Data;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUser;
using Polly;
using Polly.Timeout;
using Serilog;

namespace BSOFT.Infrastructure.Repositories.Users
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

            // // Fallback policy: Return an empty list in case of an error
            // _fallbackPolicy = Policy<List<User>>.Handle<Exception>()
            //     .FallbackAsync(new List<User>());
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
              const string query = "SELECT * FROM AppSecurity.Users";

            var policyWrap = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);

            return await policyWrap.ExecuteAsync(async () =>
            {
                // Execute the Dapper query with Polly policies
                return (await _dbConnection.QueryAsync<User>(query)).ToList();
            });
        // const string query = "SELECT * FROM AppSecurity.Users";
        // return (await _dbConnection.QueryAsync<User>(query)).ToList();
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
                const string query = "SELECT * FROM AppSecurity.Users WHERE UserId = @UserId";

            var policyWrap = Policy.WrapAsync( _retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);

            return await policyWrap.ExecuteAsync(async () =>
            {
                // Execute the Dapper query with Polly policies
                return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { userId });
            });
            // const string query = "SELECT * FROM AppSecurity.Users WHERE UserId = @UserId";
            // return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { userId });
        }

   
        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            const string query = "SELECT * FROM AppSecurity.Users WHERE UserName = @Username";

            var policyWrap = Policy.WrapAsync( _retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);

            return await policyWrap.ExecuteAsync(async () =>
            {
                // Execute the Dapper query with Polly policies
                return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
            });
            // if (string.IsNullOrWhiteSpace(username))
            // {
            //     throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            // }

            // const string query = "SELECT * FROM AppSecurity.Users WHERE UserName = @Username";
            // return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
        }
        // public async Task<User?> GetByUsernameAsync(string username)
        // {
        //     if (string.IsNullOrWhiteSpace(username))
        //     {
        //         throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        //     }

        //     const string query = @"
        //         SELECT u.*, ur.*
        //         FROM AppSecurity.Users u
        //         LEFT JOIN AppSecurity.UserRoles ur ON u.RoleId = ur.Id
        //         WHERE u.UserName = @Username";

        //     var result = await _dbConnection.QueryAsync<User, UserRole, User>(
        //         query,
        //         (user, userRole) =>
        //         {
        //             user.UserRoles = new List<UserRole> { userRole }; // Map the UserRole to the User entity
        //             return user;
        //         },
        //         new { Username = username },
        //         splitOn: "Id"
        //     );

        //     return result.FirstOrDefault();
        // }

                public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            const string query = "SELECT 'Admin' as RoleName FROM AppSecurity.Users u WHERE u.UserId = @UserId";

            var policyWrap = Policy.WrapAsync( _retryPolicy, _circuitBreakerPolicy, _timeoutPolicy);

            return await policyWrap.ExecuteAsync(async () =>
            {
                // Execute the Dapper query with Polly policies
                return (await _dbConnection.QueryAsync<string>(query, new { UserId = userId })).ToList();
            });
            //     // const string query = @"
            //     // SELECT ur.RoleName
            //     // FROM AppSecurity.UserRole ur
            //     // INNER JOIN AppSecurity.Users u ON ur.Id = u.UserRoleId
            //     // WHERE u.UserId = @UserId";
            //     const string query = @"
            //     SELECT 'Admin' as RoleName FROM AppSecurity.Users u 
            //     WHERE u.UserId = @UserId";
            // return (await _dbConnection.QueryAsync<string>(query, new { UserId = userId })).ToList();

        }
        
    }
}
