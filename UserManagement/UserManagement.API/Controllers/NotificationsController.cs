using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Infrastructure.Data;
using Core.Application.Notification.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ApiControllerBase
    {
         private readonly NotificationsQueryHandler _NotificationsQueryHandler;
         private readonly IMediator _mediator;
         private readonly ApplicationDbContext _dbContext;
         private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(NotificationsQueryHandler NotificationsQueryHandler, IMediator mediator, ApplicationDbContext dbContext, ILogger<NotificationsController> logger) 
        : base(mediator)
        {
            _NotificationsQueryHandler = NotificationsQueryHandler;   
            _dbContext = dbContext; 
            _mediator = mediator; 
            _logger = logger;
        }
        [HttpPost("PasswordResetNotifications")]
        public async Task<IActionResult> PasswordResetNotifications([FromBody] NotificationRequest request)
        {
            var response = await _NotificationsQueryHandler.Handle(request, CancellationToken.None);
            if (response.IsSuccess==false)
            {
                _logger.LogInformation("User {Username} Password Reset Notifications information.", request.Username);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = response.Message
                  
                });
            }

            _logger.LogWarning("Password is still valid for Username: {Username}. Reason: {Message}", 
                request.Username, response.Message);
           
            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = response.Message
            });
        }      
    }
}