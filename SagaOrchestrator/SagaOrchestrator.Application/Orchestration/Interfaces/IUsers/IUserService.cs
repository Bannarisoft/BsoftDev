using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.Users;

namespace SagaOrchestrator.Application.Orchestration.Interfaces.IUsers
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int userId, string token);
    }
}