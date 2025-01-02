using Dapper;
using System.Data;
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Infrastructure.Repositories;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
         private readonly IDbConnection _dbConnection;

        public UserRepository(ApplicationDbContext applicationDbContext,IDbConnection dbConnection)
        {
            _applicationDbContext = applicationDbContext;
             _dbConnection = dbConnection;
        }

        public async Task<User> CreateAsync(User user)
        {
            await _applicationDbContext.User.AddAsync(user);
            await _applicationDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<int> DeleteAsync(int userId,User user)
        {
            var existingUser = await _applicationDbContext.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existingUser != null)
            {
                existingUser.IsActive = user.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
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

        public async Task<int> UpdateAsync(int userId, User user)
        {
            var existingUser = await _applicationDbContext.User.FirstOrDefaultAsync(u => u.UserId == userId);
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
                existingUser.UserRoleId = user.UserRoleId;
                existingUser.IsFirstTimeUser = user.IsFirstTimeUser;

                _applicationDbContext.User.Update(existingUser);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
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
