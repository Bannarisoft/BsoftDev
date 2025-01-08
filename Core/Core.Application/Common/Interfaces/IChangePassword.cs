using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces
{
    public interface IChangePassword
    {
        Task<PasswordLog> ChangePassword(int userId,PasswordLog passwordLog);
        Task<bool> CheckPassword(PasswordLog passwordLog);
        Task<string> PasswordEncode(string password);
    }
}