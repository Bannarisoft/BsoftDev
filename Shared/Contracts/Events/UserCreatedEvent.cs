using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public class UserCreatedEvent
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}