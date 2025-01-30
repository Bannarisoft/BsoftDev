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

          return users.Distinct().ToList();
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

             return userResponse.FirstOrDefault();
            // const string query = "SELECT * FROM AppSecurity.Users WHERE UserId = @UserId";
            // return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { userId });
        }

   
        public async Task<User?> GetByUsernameAsync(string username, int? id = null)
        {
             ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));

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

             return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, parameters);
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
                const string query = @"
                SELECT ur.RoleName
                FROM AppSecurity.UserRole ur
                INNER JOIN AppSecurity.UserRoleAllocation ura ON   ur.Id = ura.UserRoleId
                INNER JOIN AppSecurity.Users u ON u.UserId = ura.UserId
                WHERE u.UserId = @UserId and u.IsDeleted = 0";
                // const string query = @"
                // SELECT 'Admin' as RoleName FROM AppSecurity.Users u 
                // WHERE u.UserId = @UserId";
            return (await _dbConnection.QueryAsync<string>(query, new { UserId = userId })).ToList();

        }
        
    }
}
