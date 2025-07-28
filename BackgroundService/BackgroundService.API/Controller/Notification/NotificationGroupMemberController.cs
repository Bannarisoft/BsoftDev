using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.NotificationGroupMember.Commands.CreateNotificationGroupMember;
using BackgroundService.Application.Notification.NotificationGroupMember.Commands.DeleteNotificationGroupMember;
using BackgroundService.Application.Notification.NotificationGroupMember.Commands.UpdateNotificationGroupMember;
using BackgroundService.Application.Notification.NotificationGroupMember.Queries.GetAllNotificationGroupMember;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundService.API.Controller.Notification
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationGroupMemberController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationGroupMemberController(IMediator mediator)
        : base(mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNotificationGroupMemberAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var Notification = await Mediator.Send(
             new GetAllNotificationGroupMembersQuery
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
        public async Task<IActionResult> CreateAsync(CreateNotificationGroupMemberCommand createNotificationGroupCommand)
        {            
            var CreatedNotificationId = await _mediator.Send(createNotificationGroupCommand);            
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message ="Created successfully.",
                data = CreatedNotificationId
            });            
        
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateNotificationGroupMemberCommand updateNotificationGroupMemberCommand)
        {
            await _mediator.Send(updateNotificationGroupMemberCommand);            
            return Ok(new
            {
                message = "Updated successfully.",
                statusCode = StatusCodes.Status200OK
            });                
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _mediator.Send(new DeleteNotificationGroupMemberCommand { Id = id });
            return Ok(new
            {
                message = "Deleted successfully.",
                statusCode = StatusCodes.Status200OK
            });
        
        }
    }
}