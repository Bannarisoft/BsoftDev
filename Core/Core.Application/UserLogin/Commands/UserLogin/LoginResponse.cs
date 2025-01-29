using Core.Application.Users.Queries.GetUsers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.UserLogin.Commands.UserLogin
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? UserName { get; set; }
        // public string UserRole { get; set; }
        // public List<string> UserRole { get; set; }
        public IEnumerable<string> UserRole { get; set; } = new List<string>();
        public bool IsAuthenticated { get; set; }
        public bool IsFirstTimeUser { get; set; }

        public string? Message { get; set; }   
        public int CompanyId { get; set; }     
    }
}