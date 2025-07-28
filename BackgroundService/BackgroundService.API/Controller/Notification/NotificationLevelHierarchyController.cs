using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.CreateNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.DeleteNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.UpdateNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetAllNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetNotificationLevelHierarchyById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundService.API.Controller.Notification
{
     [Route("api/[controller]")]
    public class NotificationLevelHierarchyController :  ApiControllerBase
    {        
        private readonly IMediator _mediator;
        public NotificationLevelHierarchyController(IMediator mediator)
        : base(mediator)
        {            
            _mediator=mediator;
        }        
        [HttpGet]
        public async Task<IActionResult> GetAllNotificationLevelHierarchyAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var NotificationLevelHierarchy = await Mediator.Send(
            new GetAllNotificationLevelHierarchyQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = NotificationLevelHierarchy.Data,
                TotalCount = NotificationLevelHierarchy.TotalCount,
                PageNumber = NotificationLevelHierarchy.PageNumber,
                PageSize = NotificationLevelHierarchy.PageSize
                });
        }        
      
        [HttpGet("{id}")]        
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var NotificationLevelHierarchy = await Mediator.Send(new GetNotificationLevelHierarchyByIdQuery() { Id = id});           
            return Ok(new { StatusCode=StatusCodes.Status200OK, data = NotificationLevelHierarchy,message = NotificationLevelHierarchy });            
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateNotificationLevelHierarchyCommand createNotificationLevelHierarchyCommand)
        {            
            var CreatedNotificationId = await _mediator.Send(createNotificationLevelHierarchyCommand);            
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message ="Created successfully.",
                data = CreatedNotificationId
            });            
        
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateNotificationLevelHierarchyCommand updateNotificationLevelHierarchyCommand)
        {
            await _mediator.Send(updateNotificationLevelHierarchyCommand);            
            return Ok(new
            {
                message = "Updated successfully.",
                statusCode = StatusCodes.Status200OK
            });                
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _mediator.Send(new DeleteNotificationLevelHierarchyCommand { Id = id });
            return Ok(new
            {
                message = "Deleted successfully.",
                statusCode = StatusCodes.Status200OK
            });        
        }                
    }
}