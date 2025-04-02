using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public interface IUserCreatedEvent
    {
        Guid CorrelationId { get; }
        int UserId { get; }
        string Email { get; }
    }
}