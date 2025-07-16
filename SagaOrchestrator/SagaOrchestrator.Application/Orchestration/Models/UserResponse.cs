using Contracts.Dtos.Users;

namespace SagaOrchestrator.Application.Models
{
    public class UserResponse
    {
        public int StatusCode { get; set; }
        public UserDto? Data { get; set; }
    }
}