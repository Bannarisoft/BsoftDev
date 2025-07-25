using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.CreateNotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.DeleteNotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.UpdateNotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Queries.GetAllNotificationEventRule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundService.API.Controller.Notification
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationEventRuleController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationEventRuleController(IMediator mediator)
        : base(mediator)
        {
            _mediator = mediator;
        }
         [HttpGet]
        public async Task<IActionResult> GetAllNotificationGroupMemberAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var Notification = await Mediator.Send(
             new GetAllNotificationEventRuleQuery
             {
                 PageNumber = PageNumber,
                 PageSize = PageSize,
                 SearchTerm = SearchTerm
             });
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = Notification.Data,
                TotalCount = Notification.TotalCount,
                PageNumber = Notification.PageNumber,
                PageSize = Notification.PageSize
            });
        }
           [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateNotificationEventRuleCommand createNotificationGroupMemberCommand)
        {            
            var CreatedNotificationId = await _mediator.Send(createNotificationGroupMemberCommand);            
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message ="Created successfully.",
                data = CreatedNotificationId
            });            
        
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateNotificationEventRuleCommand updateNotificationEventRuleCommand)
        {
            await _mediator.Send(updateNotificationEventRuleCommand);            
            return Ok(new
            {
                message = "Updated successfully.",
                statusCode = StatusCodes.Status200OK
            });                
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _mediator.Send(new DeleteNotificationEventRuleCommand { Id = id });
            return Ok(new
            {
                message = "Deleted successfully.",
                statusCode = StatusCodes.Status200OK
            });
        
        }
    }
}