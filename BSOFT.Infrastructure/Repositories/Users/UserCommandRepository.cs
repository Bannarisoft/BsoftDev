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
using Core.Application.Common.Interfaces.IUser;

namespace BSOFT.Infrastructure.Repositories
{
     public class UserCommandRepository : IUserCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
         private readonly IDbConnection _dbConnection;

        
		public UserCommandRepository(ApplicationDbContext applicationDbContext,IDbConnection dbConnection)
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
                // existingUser.UserRoleId = user.UserRoleId;
                existingUser.IsFirstTimeUser = user.IsFirstTimeUser;

                _applicationDbContext.User.Update(existingUser);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }

        // public async Task<User?> GetByUsernameAsync(string searchPattern)
        // {
        //         const string query = "SELECT * FROM AppSecurity.Users WHERE UserName LIKE @SearchPattern";
        //         return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { SearchPattern = $"%{searchPattern}%" });
        // }
        public async Task<User?> GetByUsernameAsync(string username)
        {
            const string query = "SELECT * FROM AppSecurity.Users WHERE UserName = @Username";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
        }
        
        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
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
