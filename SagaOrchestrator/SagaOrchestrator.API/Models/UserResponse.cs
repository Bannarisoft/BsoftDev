using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.Users;

namespace SagaOrchestrator.API.Models
{
    public class UserResponse
    {
        public int StatusCode { get; set; }
        public UserDto Data { get; set; }
    }
}