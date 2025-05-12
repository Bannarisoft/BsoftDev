using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityCheckListMaster.Command.UpdateActivityCheckListMaster;
using Core.Application.ActivityMaster.Command.CreateActivityMaster;
using Core.Application.ActivityMaster.Command.UpdateActivityMster;
using Core.Application.ActivityMaster.Queries.GetActivityType;
using Core.Application.ActivityMaster.Queries.GetAllActivityMaster;
using Core.Application.ActivityMaster.Queries.GetMachineGroupById;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Application.MachineGroup.Queries.GetActivityMasterAutoComplete;
using Core.Application.MachineGroup.Queries.GetMachineGroupById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
     [Route("api/[controller]")]
    public class ActivityMasterController : ApiControllerBase
    {
        private readonly IActivityMasterCommandRepository _activityMasterCommandRepository;

         private  readonly IValidator<CreateActivityMasterCommand>  _createactivityMasterCommandValidator;
         private readonly IValidator<UpdateActivityMasterCommand>  _updateActivityMasterCommandValidator;

        public ActivityMasterController(ISender mediator ,IActivityMasterCommandRepository activityMasterCommandRepository ,IValidator<CreateActivityMasterCommand> createactivityMasterCommandValidator ,IValidator<UpdateActivityMasterCommand> updateActivityMasterCommandValidator   ):base(mediator)
        {
            _activityMasterCommandRepository = activityMasterCommandRepository;
            _createactivityMasterCommandValidator = createactivityMasterCommandValidator;
            _updateActivityMasterCommandValidator = updateActivityMasterCommandValidator;
          
        }

            [HttpGet] 
          public async Task<IActionResult> GetAllActivityMasterAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
            var activitymaster = await Mediator.Send(
            new GetAllActivityMasterQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           // var activecompanies = companies.Data.ToList(); 

            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = activitymaster.Data,
                TotalCount = activitymaster.TotalCount,
                PageNumber = activitymaster.PageNumber,
                PageSize = activitymaster.PageSize
            });
        }



         [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
           
            var activitymaster = await Mediator.Send(new GetActivityMasterByIdQuery() { Id = id});
          
             if(activitymaster.IsSuccess)
            {
                 return Ok(new { StatusCode=StatusCodes.Status200OK, data = activitymaster.Data,message=activitymaster.Message});
            }

            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = $"Activity Group master ID {id} not found.", errors = "" });
           
        }
         [HttpGet("MachineGroup/{activityId}")]
         [ActionName(nameof(GetMachineGroupById))]
         public async Task<IActionResult> GetMachineGroupById(int activityId)
         {
             var MachineGroup = await Mediator.Send(new GetMachineGroupNameByIdQuery() { ActivityId = activityId});
          
             if(MachineGroup.IsSuccess)
            {
                 return Ok(new { StatusCode=StatusCodes.Status200OK, data = MachineGroup.Data,message=MachineGroup.Message});
            }

            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = $"Activity Machine Group master ID {activityId} not found.", errors = "" });

         }

         [HttpGet("by-name")]
        public async Task<IActionResult> GetMachineGroup([FromQuery] string? name)
        {
          
            var activitymaster = await Mediator.Send(new GetActivityMasterAutoCompleteQuery {SearchPattern = name});
            if(activitymaster.IsSuccess)
            {
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = activitymaster.Data });
            }
            return NotFound( new { StatusCode=activitymaster.Message}) ;
        }
        
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateActivityMasterCommand command)
        {
             var validationResult = await _createactivityMasterCommandValidator.ValidateAsync(command);
            
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

        [HttpPut("update")]
        public async Task<IActionResult> UpdateActivityMaster([FromBody] UpdateActivityMasterCommand command)
        {

              var validationResult = await _updateActivityMasterCommandValidator.ValidateAsync(command);
            if (command == null)
            {
                return BadRequest(new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Invalid request data."
                });
            }

            var result = await Mediator.Send(command);

                        if (!result.IsSuccess)
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = result.Message,
                        errors = "" 
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = result.Message,
                    errors = "" 
                });
          }


              [HttpGet("GetActivityType")]
                public async Task<IActionResult> GetActivityTypeAsync()
                {
                    var result = await Mediator.Send(new GetActivityTypeQuery());
                    if (result == null || result.Data == null || result.Data.Count == 0)
                    {
                        return NotFound(new
                        {
                            StatusCode = StatusCodes.Status404NotFound,
                            message = "No ActivityType  found."
                        });
                    }
                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        message = "ActivityType  fetched successfully.",
                        data = result.Data
                    });
                }






    }

}