using Dapper;
using System.Data;
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Infrastructure.Repositories;
using Core.Domain.Entities;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUser;

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

        // public async Task<List<User>> GetAllUsersAsync()
        // {
        //     return await _applicationDbContext.User.ToListAsync();
        // }
        public async Task<List<User>> GetAllUsersAsync()
        {
        const string query = "SELECT * FROM AppSecurity.Users";
        return (await _dbConnection.QueryAsync<User>(query)).ToList();
        }

        // public async Task<User?> GetByIdAsync(int userId)
        // {
        //     return await _applicationDbContext.User.AsNoTracking()
        //         .FirstOrDefaultAsync(b => b.UserId == userId);
        // }
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
            var user = await _applicationDbContext.User
                .Include(u => u.UserRoles) // Ensure UserRoles are included
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Extract RoleName from UserRoles collection
            var roles = user.UserRoles.Select(ur => ur.RoleName).ToList();
            return roles;
        }
        
    }
}
