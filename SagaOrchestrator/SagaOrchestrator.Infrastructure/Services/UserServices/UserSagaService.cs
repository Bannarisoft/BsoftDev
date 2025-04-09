using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Users;
using Contracts.Models.Users;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;

namespace SagaOrchestrator.Infrastructure.Services.UserServices
{
    public class UserSagaService
    {
        private readonly IUserService _userService;
        private readonly IPublishEndpoint _publishEndpoint;
        public UserSagaService(IUserService userService, IPublishEndpoint publishEndpoint)
        {
            _userService = userService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<UserDto> TriggerUserCreation(int userId, string token)
        {
            var user = await _userService.GetUserByIdAsync(userId, token);

            if (user != null)
            {
                var userCreatedEvent = new UserCreatedEvent
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email
                };
                await _publishEndpoint.Publish(userCreatedEvent);
                return user;
            }
            return null;
        }


    }
}