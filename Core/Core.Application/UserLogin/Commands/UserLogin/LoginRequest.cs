using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.UserLogin.Commands.UserLogin
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}