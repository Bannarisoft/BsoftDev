using System.Security.Cryptography;
using BCrypt.Net;
using Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class User : BaseEntity
    {
    public Guid Id { get; set; }
    public int UserId { get; set; }// Identity column
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public  bool IsActive { get; set; }
    public bool IsFirstTimeUser { get; set; } = false;
    public string PasswordHash { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int CompanyId { get; set; }
    public int UnitId { get; set; }
    public int DivisionId { get; set; }
    public int UserRoleId { get; set; }
     public ICollection<UserRole> UserRoles { get; set; }

    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        // Generate a valid BCrypt hash
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }
// public static string HashPassword(string password, byte[] salt)
// {
//     using (var sha256 = SHA256.Create())
//     {
//         var inputBytes = Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt));
//         var hashedBytes = sha256.ComputeHash(inputBytes);
//         return Convert.ToBase64String(hashedBytes);
//     }
// }
// public static bool VerifyPassword(string password, string hashedPassword, byte[] salt)
// {
//     var computedHash = HashPassword(password, salt);
//     return computedHash == hashedPassword;
// }
    }
}