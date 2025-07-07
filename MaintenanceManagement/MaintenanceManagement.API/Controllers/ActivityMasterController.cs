using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityCheckListMaster.Command.UpdateActivityCheckListMaster;
using Core.Application.ActivityMaster.Command.CreateActivityMaster;
using Core.Application.ActivityMaster.Command.UpdateActivityMster;
using Core.Application.ActivityMaster.Queries.GetActivityByMachinGroupId;
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
        
        public ActivityMasterController(ISender mediator ):base(mediator)
        {
            
          
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
          
           
                 return Ok(new { StatusCode=StatusCodes.Status200OK, data = activitymaster,message="" });
            
           
        }
         [HttpGet("MachineGroup/{activityId}")]
         [ActionName(nameof(GetMachineGroupById))]
         public async Task<IActionResult> GetMachineGroupById(int activityId)
         {
             var MachineGroup = await Mediator.Send(new GetMachineGroupNameByIdQuery() { ActivityId = activityId});
          
            
                 return Ok(new { StatusCode=StatusCodes.Status200OK, data = MachineGroup,message="" });


         }

         [HttpGet("by-name")]
        public async Task<IActionResult> GetMachineGroup([FromQuery] string? name)
        {
          
            var activitymaster = await Mediator.Send(new GetActivityMasterAutoCompleteQuery {SearchPattern = name});
          
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = activitymaster });
        
        }
        
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateActivityMasterCommand command)
        {
            //  var validationResult = await _createactivityMasterCommandValidator.ValidateAsync(command);
            
            // if (!validationResult.IsValid)
            // {
            //     return BadRequest(new 
            //     {
            //         StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
            //         errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
            //     });
            // }
          
            var response = await Mediator.Send(command);
                                       
                return Ok(new 
                {
                     StatusCode=StatusCodes.Status201Created,
                 message = response,
                  errors = "",
                  data = response 
                  });
                        
        } 

        [HttpPut("update")]
        public async Task<IActionResult> UpdateActivityMaster([FromBody] UpdateActivityMasterCommand command)
        {

            //   var validationResult = await _updateActivityMasterCommandValidator.ValidateAsync(command);
            // if (command == null)
            // {
            //     return BadRequest(new ApiResponseDTO<int>
            //     {
            //         IsSuccess = false,
            //         Message = "Invalid request data."
            //     });
            // }

            var result = await Mediator.Send(command);

                //         if (!result.IsSuccess)
                // {
                //     return BadRequest(new
                //     {
                //         StatusCode = StatusCodes.Status400BadRequest,
                //         Message = result.Message,
                //         errors = "" 
                //     });
                // }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = result,
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


        [HttpGet("GetActivity/{machineGroupId}")]
        public async Task<IActionResult> GetActivityByMachineGroupId(int machineGroupId)
        {
            var response = await Mediator.Send(new GetActivityByMachinGroupIdQuery
            {
                MachineGroupId = machineGroupId
            });

            // if (!response.IsSuccess || response.Data == null || !response.Data.Any())
            // {
            //     return NotFound(new
            //     {
            //         StatusCode = StatusCodes.Status404NotFound,
            //         Message = $"No activities found for Machine Group ID {machineGroupId}.",
            //         Errors = string.Empty
            //     });
            // }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = response,
                Data = response
            });
        }




    }

}