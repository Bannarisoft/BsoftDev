using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityCheckListMaster.Command.CreateActivityCheckListMaster;
using Core.Application.ActivityCheckListMaster.Command.UpdateActivityCheckListMaster;
using Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMaster;
using Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMasterById;
using Core.Application.ActivityCheckListMaster.Queries.GetCheckListByActivityId;
using Core.Application.Common.Interfaces.IActivityMaster;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("[controller]")]
    public class ActivityCheckListMasterController : ApiControllerBase
    {
       private readonly IActivityMasterCommandRepository _activityMasterCommandRepository;


       private  readonly IValidator<CreateActivityCheckListMasterCommand> _createActivityCheckListCommand;
       private readonly IValidator<UpdateActivityCheckListMasterCommand> _updateActivityCheckListCommand;



        public ActivityCheckListMasterController(ISender mediator ,IActivityMasterCommandRepository activityMasterCommandRepository, IValidator<CreateActivityCheckListMasterCommand>  createActivityCheckListCommand ,
        IValidator<UpdateActivityCheckListMasterCommand>  updateActivityCheckListCommand   ) :base(mediator)
        {
            _activityMasterCommandRepository = activityMasterCommandRepository;
            _createActivityCheckListCommand = createActivityCheckListCommand;
            _updateActivityCheckListCommand = updateActivityCheckListCommand;

        }

 
          [HttpGet] 
          public async Task<IActionResult> GetAllActivityCheckListMasterAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
            var CheckListMaster = await Mediator.Send(
            new GetAllActivityCheckListMasterQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           // var activecompanies = companies.Data.ToList(); 

            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = CheckListMaster.Data,
                TotalCount = CheckListMaster.TotalCount,
                PageNumber = CheckListMaster.PageNumber,
                PageSize = CheckListMaster.PageSize
            });
        }

         [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var CheckListMaster = await Mediator.Send(new GetActivityCheckListMasterByIdQuery() { Id = id});
          
            if(CheckListMaster.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = CheckListMaster.Data,message = CheckListMaster.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = CheckListMaster.Message });   
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateActivityCheckListMasterCommand command)
        {
             var validationResult = await _createActivityCheckListCommand.ValidateAsync(command);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
          
            var response = await Mediator.Send(command);
            if(response.IsSuccess)
            {                             
                return Ok(new 
                {
                     StatusCode=StatusCodes.Status201Created,
                 message = response.Message,
                  errors = "",
                  data = response.Data 
                  });
            }
             
            
            return BadRequest( new {
                 StatusCode=StatusCodes.Status400BadRequest,
                  message = response.Message, 
                  errors = "" 
                  }); 
            
        } 

            [HttpPut] 
         public async Task<IActionResult> Update(UpdateActivityCheckListMasterCommand command)
        {
            // Update the record
              var validationResult = await _updateActivityCheckListCommand.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                 return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }

            var response = await Mediator.Send(command);
            if (response.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode = StatusCodes.Status200OK, 
                    Message = response.Message, 
                    Errors = "" 
                });
            }

            // If update failed
            return BadRequest(new 
            { 
                StatusCode = StatusCodes.Status400BadRequest, 
                Message = response.Message, 
                Errors = "" 
            });
        }
         [HttpGet("ByActivityId/{id}")]
        [ActionName(nameof(GetCheckListByActivityIdAsync))]
        public async Task<IActionResult> GetCheckListByActivityIdAsync(int id)
        {
           
            var activityCheckList = await Mediator.Send(new GetActivityCheckListByActivityIdQuery() { Id = id});
          
             if(activityCheckList.IsSuccess)
            {
                 return Ok(new { StatusCode=StatusCodes.Status200OK, data = activityCheckList.Data,message=activityCheckList.Message});
            }

            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = $"ActivityCheckList ID {id} not found.", errors = "" });
           
        }

        




    }
}
