using Core.Application.Item.ItemGroup.Commands.CreateItemGroup;
using Core.Application.Item.ItemGroup.Commands.DeleteItemGroup;
using Core.Application.Item.ItemGroup.Commands.UpdateItemGroup;
using Core.Application.Item.ItemGroup.Queries.GetItemGroup;
using Core.Application.Item.ItemGroup.Queries.GetItemGroupAutoComplete;
using Core.Application.Item.ItemGroup.Queries.GetItemGroupById;
using InventoryManagement.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controller.Item
{
     [Route("api/[controller]")]
    public class ItemGroupController :  ApiControllerBase
    {        
        private readonly IMediator _mediator;
        public ItemGroupController(IMediator mediator)
        : base(mediator)
        {            
            _mediator=mediator;
        }        
        [HttpGet]
        public async Task<IActionResult> GetAllItemGroupAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var notificationConfig = await Mediator.Send(
            new GetItemGroupQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = notificationConfig.Data,
                TotalCount = notificationConfig.TotalCount,
                PageNumber = notificationConfig.PageNumber,
                PageSize = notificationConfig.PageSize
                });
        }
        
        [HttpGet("by-name")]
        public async Task<IActionResult> GetItemGroupAutoCompleteAsync([FromQuery] string? ModuleName)
        {
            var notificationConfig = await Mediator.Send(new GetItemGroupAutoCompleteQuery 
            { 
                    SearchPattern = ModuleName ?? string.Empty 
            });
            return Ok(new { StatusCode = StatusCodes.Status200OK, data = notificationConfig});
        }

        [HttpGet("{id}")]        
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var notificationConfig = await Mediator.Send(new GetItemGroupByIdQuery() { Id = id});           
            return Ok(new { StatusCode=StatusCodes.Status200OK, data = notificationConfig,message = notificationConfig });            
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateItemGroupCommand createItemGroupCommand)
        {            
            var CreatedNotificationId = await _mediator.Send(createItemGroupCommand);            
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message ="Created successfully.",
                data = CreatedNotificationId
            });            
        
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateItemGroupCommand updateNotificationConfigCommand)
        {
            await _mediator.Send(updateNotificationConfigCommand);            
            return Ok(new
            {
                message = "Updated successfully.",
                statusCode = StatusCodes.Status200OK
            });                
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _mediator.Send(new DeleteItemGroupCommand { Id = id });
            return Ok(new
            {
                message = "Deleted successfully.",
                statusCode = StatusCodes.Status200OK
            });
        
        }
                
    }
}