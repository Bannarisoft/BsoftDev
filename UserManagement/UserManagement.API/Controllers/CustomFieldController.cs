using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.CustomFields.Commands.CreateCustomField;
using Core.Application.CustomFields.Commands.DeleteCustomField;
using Core.Application.CustomFields.Commands.UpdateCustomField;
using Core.Application.CustomFields.Queries.GetCustomField;
using Core.Application.CustomFields.Queries.GetCustomFieldById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomFieldController : ApiControllerBase
    {
        private readonly IValidator<CreateCustomFieldCommand> _createCustomFieldCommandValidator;
        private readonly IValidator<UpdateCustomFieldCommand> _updateCustomFieldCommandValidator;
        private readonly IValidator<DeleteCustomFieldCommand> _deleteCustomFieldCommandValidator;
        public CustomFieldController(ISender mediator, 
                                    IValidator<CreateCustomFieldCommand> createCustomFieldCommandValidator,
                                    IValidator<UpdateCustomFieldCommand> updateCustomFieldCommandValidator,
                                    IValidator<DeleteCustomFieldCommand> deleteCustomFieldCommandValidator) 
        : base(mediator)
        {
            _createCustomFieldCommandValidator = createCustomFieldCommandValidator;
            _updateCustomFieldCommandValidator = updateCustomFieldCommandValidator;
            _deleteCustomFieldCommandValidator = deleteCustomFieldCommandValidator;
        }
           [HttpGet]
        public async Task<IActionResult> GetAllCustomFieldsAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var divisions = await Mediator.Send(
            new GetCustomFieldQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = divisions.Data,
                TotalCount = divisions.TotalCount,
                PageNumber = divisions.PageNumber,
                PageSize = divisions.PageSize
                });
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCustomFieldCommand command)
        {
            
            var validationResult = await _createCustomFieldCommandValidator.ValidateAsync(command);
            
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
         [HttpGet("{id}")]
         [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
               
            var customField = await Mediator.Send(new GetCustomFieldByIdQuery { Id = id});
          
             if(customField == null)
            {
                return NotFound( new 
                { 
                    StatusCode=StatusCodes.Status404NotFound, 
                    message = $"CustomField ID {id} not found.", 
                    errors = "" 
                });
            }
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = customField.Data
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update( UpdateCustomFieldCommand command )
        {
            var validationResult = await _updateCustomFieldCommandValidator.ValidateAsync(command);
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
            var command = new DeleteCustomFieldCommand { Id = id };
             var validationResult = await  _deleteCustomFieldCommandValidator.ValidateAsync(command);
               if (!validationResult.IsValid)
                {
                    return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
                }
           var updatedCustomField = await Mediator.Send(command);

           if(updatedCustomField.IsSuccess)
           {
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = updatedCustomField.Message, 
                errors = "" 
            });
              
           }

            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = updatedCustomField.Message, 
                errors = "" 
            });
            
        }
    }
}