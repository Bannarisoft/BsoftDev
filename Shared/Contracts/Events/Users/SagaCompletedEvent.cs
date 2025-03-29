using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Events.Users
{
    public class SagaCompletedEvent
    {
        public int UserId { get; set; }
        public string? Status { get; set; }
    }
}