using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PreventiveSchedulers.Commands.ActiveInActivePreventive;
using Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.DeletePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.ReschedulePreventive;
using Core.Application.PreventiveSchedulers.Commands.ScheduleWorkOrder;
using Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Queries.GetDetailSchedulerByDate;
using Core.Application.PreventiveSchedulers.Queries.GetPreventiveScheduler;
using Core.Application.PreventiveSchedulers.Queries.GetPreventiveSchedulerById;
using Core.Application.PreventiveSchedulers.Queries.GetSchedulerByDate;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreventiveSchedulerController : ApiControllerBase
    {
        private readonly IValidator<CreatePreventiveSchedulerCommand> _createPreventiveSchedulerCommand;
        private readonly IValidator<UpdatePreventiveSchedulerCommand> _updatePreventiveSchedulerCommand;
        private readonly IValidator<DeletePreventiveSchedulerCommand> _deletePreventiveSchedulerCommand;
        private readonly IValidator<ActiveInActivePreventiveCommand> _activeInActivePreventiveCommand;
        public PreventiveSchedulerController(ISender mediator,IValidator<CreatePreventiveSchedulerCommand> createPreventiveSchedulerCommand,IValidator<UpdatePreventiveSchedulerCommand> updatePreventiveSchedulerCommand,IValidator<DeletePreventiveSchedulerCommand> deletePreventiveSchedulerCommand,IValidator<ActiveInActivePreventiveCommand> activeInActivePreventiveCommand) 
        : base(mediator)
        {
            _createPreventiveSchedulerCommand = createPreventiveSchedulerCommand;
            _updatePreventiveSchedulerCommand = updatePreventiveSchedulerCommand;
            _deletePreventiveSchedulerCommand = deletePreventiveSchedulerCommand;
            _activeInActivePreventiveCommand = activeInActivePreventiveCommand;
        }
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var response = await Mediator.Send(
            new GetPreventiveSchedulerQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = response.Data,
                TotalCount = response.TotalCount,
                PageNumber = response.PageNumber,
                PageSize = response.PageSize
                });
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreatePreventiveSchedulerCommand command)
        {
            
            var validationResult = await _createPreventiveSchedulerCommand.ValidateAsync(command);
            
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
             

            return BadRequest( new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = response.Message, 
                errors = "" 
            }); 
            
        }
         [Route("[action]/{id}")]
         [HttpGet]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
               
            var preventiveScheduler = await Mediator.Send(new GetPreventiveSchedulerByIdQuery { Id = id});
          
             if(preventiveScheduler == null)
            {
                return NotFound( new 
                { 
                    StatusCode=StatusCodes.Status404NotFound, 
                    message = $"PreventiveScheduler ID {id} not found.", 
                    errors = "" 
                });
            }
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = preventiveScheduler.Data
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update( UpdatePreventiveSchedulerCommand command )
        {
            var validationResult = await _updatePreventiveSchedulerCommand.ValidateAsync(command);
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
                    StatusCode=StatusCodes.Status200OK, 
                    message = response.Message, 
                    errors = "" 
                });
             }
            
           

            return BadRequest( new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = response.Message, 
                errors = "" 
            }); 
        }


        [HttpDelete("{id}")]
        
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeletePreventiveSchedulerCommand { Id = id };
             var validationResult = await  _deletePreventiveSchedulerCommand.ValidateAsync(command);
               if (!validationResult.IsValid)
                {
                    return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
                }
           var updatedPreventiveScheduler = await Mediator.Send(command);

           if(updatedPreventiveScheduler.IsSuccess)
           {
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = updatedPreventiveScheduler.Message, 
                errors = "" 
            });
              
           }

            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = updatedPreventiveScheduler.Message, 
                errors = "" 
            });
            
        }
          [HttpPut("reschedule")]
        public async Task<IActionResult> Reschedule( ReshedulePreventiveCommand command )
        {
            // var validationResult = await _updateCustomFieldCommandValidator.ValidateAsync(command);
            // if (!validationResult.IsValid)
            // {
            //      return BadRequest(new 
            //     {
            //         StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
            //         errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
            //     });
            // }

             var response = await Mediator.Send(command);
             if(response.IsSuccess)
             {
                 return Ok(new 
                 { 
                    StatusCode=StatusCodes.Status200OK, 
                    message = response.Message, 
                    errors = "" 
                });
             }
            
           

            return BadRequest( new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = response.Message, 
                errors = "" 
            }); 
        }
          [HttpPut("UpdateActiveStatus")]
        public async Task<IActionResult> UpdateActiveInActive( ActiveInActivePreventiveCommand command )
        {
            var validationResult = await _activeInActivePreventiveCommand.ValidateAsync(command);
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
                    StatusCode=StatusCodes.Status200OK, 
                    message = response.Message, 
                    errors = "" 
                });
             }
            
           

            return BadRequest( new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = response.Message, 
                errors = "" 
            }); 
        }
          [HttpGet("SchedulerAbstractByDate")]
        public async Task<IActionResult> GetScheduler([FromQuery] int DepartmentId )
        {
           var response = await Mediator.Send(
            new GetSchedulerByDateQuery
            {
                DepartmentId =DepartmentId
            });
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = response.Data
            });
        }
          [HttpPost("SchedulerDetailByDate")]
        public async Task<IActionResult> GetSchedulerDetail(GetDetailSchedulerByDateQuery command)
        {
           var response =  await Mediator.Send(command);
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = response.Data
            });
        }
         [HttpPost("HangfireSchedule")]
         [AllowAnonymous]
        public async Task<IActionResult> HangfireSchedule(ScheduleWorkOrderCommand command)
        {
           var response =  await Mediator.Send(command);
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = response.Data
            });
        }
    }
}