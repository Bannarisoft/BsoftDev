using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public interface IUserCreated
    {
        Guid UserId { get; }
        string Username { get; }
        string Email { get; }
    }
}