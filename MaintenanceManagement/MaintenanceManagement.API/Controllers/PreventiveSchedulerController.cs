using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.DeletePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Queries.GetPreventiveScheduler;
using Core.Application.PreventiveSchedulers.Queries.GetPreventiveSchedulerById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreventiveSchedulerController : ApiControllerBase
    {
        public PreventiveSchedulerController(ISender mediator) 
        : base(mediator)
        {
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
            
            // var validationResult = await _createCustomFieldCommandValidator.ValidateAsync(command);
            
            // if (!validationResult.IsValid)
            // {
            //     return BadRequest(new 
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


        [HttpDelete("{id}")]
        
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeletePreventiveSchedulerCommand { Id = id };
            //  var validationResult = await  _deleteCustomFieldCommandValidator.ValidateAsync(command);
            //    if (!validationResult.IsValid)
            //     {
            //         return BadRequest(new 
            //     {
            //         StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
            //         errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
            //     });
            //     }
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
    }
}