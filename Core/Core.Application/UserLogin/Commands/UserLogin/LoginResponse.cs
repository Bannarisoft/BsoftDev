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
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; }
        // public string UserRole { get; set; }
        public List<string> UserRole { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}