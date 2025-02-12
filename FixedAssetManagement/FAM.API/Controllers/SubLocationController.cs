using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Location.Command.DeleteAubLocation;
using Core.Application.Location.Command.UpdateSubLocation;
using Core.Application.SubLocation.Command.CreateSubLocation;
using Core.Application.SubLocation.Queries.GetSubLocationAutoComplete;
using Core.Application.SubLocation.Queries.GetSubLocationById;
using Core.Application.SubLocation.Queries.GetSubLocations;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubLocationController : ApiControllerBase
    {
        private readonly IValidator<CreateSubLocationCommand> _createSubLocationCommandValidator;
        private readonly IValidator<UpdateSubLocationCommand> _updateSubLocationCommandValidator;

    public SubLocationController(ISender mediator,IValidator<CreateSubLocationCommand> createSubLocationCommandValidator,IValidator<UpdateSubLocationCommand> updateSubLocationCommandValidator) 
        : base(mediator)
    {
            _createSubLocationCommandValidator = createSubLocationCommandValidator;
            _updateSubLocationCommandValidator = updateSubLocationCommandValidator;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllSubLocationAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
    {
        var sublocations = await Mediator.Send(
            new GetSubLocationQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = sublocations.Data.ToList(),
                TotalCount = sublocations.TotalCount,
                PageNumber = sublocations.PageNumber,
                PageSize = sublocations.PageSize
                });
        }
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateSubLocationCommand createsublocationcommand)
    {
            
        var validationResult = await _createSubLocationCommandValidator.ValidateAsync(createsublocationcommand);
            
        if (!validationResult.IsValid)
        {
            return BadRequest(new 
            {
                StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
            });
        }
            var result = await Mediator.Send(createsublocationcommand);
        if (result.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status201Created,
                    message = result.Message, 
                    data = result.Data
                });
            }                      
            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest,
                message = result.Message 
            });
            
        }
    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
         if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid SubLocation ID"
                });
            }  
           
        var result = await Mediator.Send(new GetSubLocationByIdQuery() { Id = id});
          
       if (!result.IsSuccess)
            {                
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data
            });
        }
    [HttpPut]
    public async Task<IActionResult> Update( UpdateSubLocationCommand updatesubLocationcommand )
    {
        var validationResult = await _updateSubLocationCommandValidator.ValidateAsync(updatesubLocationcommand);
        if (!validationResult.IsValid)
       {
            return BadRequest(validationResult.Errors);
       }
       var locationExists = await Mediator.Send(new GetSubLocationByIdQuery { Id = updatesubLocationcommand.Id });

        if (locationExists == null)
        {
            return NotFound(new { StatusCode=StatusCodes.Status404NotFound, message = $"SubLocation ID {updatesubLocationcommand.Id} not found.", errors = "" }); 
        }

        var result = await Mediator.Send(updatesubLocationcommand);
           if (result.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status201Created,
                    message = result.Message, 
                    data = result.Data 
                });
            }
            else
            {            
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = result.Message 
                });
            }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
         if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid SubLocation ID"
                });
            } 
           
        var deletedsublocation = await Mediator.Send(new DeleteSubLocationCommand { Id = id });

         if (!deletedsublocation.IsSuccess)
            {                
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound,
                    message = deletedsublocation.Message
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data =$"SubLocation ID {id} Deleted" 
            });
    }
    [HttpGet("by-name")]
    public async Task<IActionResult> GetSubLocation([FromQuery] string? name)
    {
        var result = await Mediator.Send(new GetSubLocationAutoCompleteQuery {SearchPattern = name});
        if (!result.IsSuccess)
            {
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message
                 }); 
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = result.Message,
                data = result.Data
            });
    }

    }
}