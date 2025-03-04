using System.Security.Cryptography;
using BCrypt.Net;
using Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Domain.Entities
{
    public class User : BaseEntity
    {
    public Guid Id { get; set; }
    public int UserId { get; set; }// Identity column
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public FirstTimeUserStatus IsFirstTimeUser { get; set; } 
    public string? PasswordHash { get; set; }
    public int? UserType { get; set; }
    public string? Mobile { get; set; }
    public string? EmailId { get; set; }
    public int? DivisionId { get; set; }
    
    public IList<UserRoleAllocation>? UserRoleAllocations { get; set; }

     public ICollection<PasswordLog>? Passwords { get; set; }
     public IList<UserCompany>? UserCompanies { get; set; }
     public IList<UserUnit> UserUnits { get; set; }
     public int? EntityId { get; set; }
     public Entity? Entity { get; set; }
     public UserGroupUsers? UserGroupUsers { get; set; }

    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        // Generate a valid BCrypt hash
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    }
}