
using BackgroundService.Application.Notification.GetNotificationDetail.GetNotificationDetailById;
using BackgroundService.Application.Notification.GetNotificationDetail.UpdateNotificationStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundService.API.Controller.Notification
{
    [Route("api/[controller]")]
    public class NotificationDetailController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationDetailController(IMediator mediator)
        : base(mediator)
        {
            _mediator = mediator;
        }
       [HttpGet("detail/{userId}")]
        public async Task<IActionResult> GetNotificationDetailByIdAsync(string userId)
        {
            var notificationDetail = await Mediator.Send(new GetNotificationDetailByUserId { UserId = userId });

            return Ok(new 
            { 
                StatusCode = StatusCodes.Status200OK, 
                Data = notificationDetail 
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateNotificationStatus updateNotificationDetail)
        {
            await _mediator.Send(updateNotificationDetail);            
            return Ok(new
            {
                message = "Updated successfully.",
                statusCode = StatusCodes.Status200OK
            });                
        }
       
    }
}

