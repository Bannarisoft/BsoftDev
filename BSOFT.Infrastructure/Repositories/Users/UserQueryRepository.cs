using Dapper;
using System.Data;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUser;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace BSOFT.Infrastructure.Repositories.Users
{
    public class UserQueryRepository : IUserQueryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
         private readonly IDbConnection _dbConnection;

        public UserQueryRepository(ApplicationDbContext applicationDbContext,IDbConnection dbConnection)
        {
            _applicationDbContext = applicationDbContext;
             _dbConnection = dbConnection;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
        const string query = "SELECT * FROM AppSecurity.Users";
        return (await _dbConnection.QueryAsync<User>(query)).ToList();
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            const string query = "SELECT * FROM AppSecurity.Users WHERE UserId = @UserId";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { userId });
        }

   
        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            const string query = "SELECT * FROM AppSecurity.Users WHERE UserName = @Username";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
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
                // const string query = @"
                // SELECT ur.RoleName
                // FROM AppSecurity.UserRole ur
                // INNER JOIN AppSecurity.Users u ON ur.Id = u.UserRoleId
                // WHERE u.UserId = @UserId";
                const string query = @"
                SELECT 'Admin' as RoleName FROM AppSecurity.Users u 
                WHERE u.UserId = @UserId";
            return (await _dbConnection.QueryAsync<string>(query, new { UserId = userId })).ToList();

        }
        
    }
}
