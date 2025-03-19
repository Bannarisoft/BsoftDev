using Core.Application.UserGroup.Commands.CreateUserGroup;
using Core.Application.UserGroup.Commands.DeleteUserGroup;
using Core.Application.UserGroup.Commands.UpdateUesrGroup;
using Core.Application.UserGroup.Queries.GetUserGroup;
using Core.Application.UserGroup.Queries.GetUserGroupAutoComplete;
using Core.Application.UserGroup.Queries.GetUserGroupById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]

    public class UserGroupController  : ApiControllerBase
    {
         private readonly IValidator<CreateUserGroupCommand> _createUserGroupCommandValidator;
         private readonly IValidator<UpdateUserGroupCommand> _updateUserGroupCommandValidator;  
         private readonly IValidator<DeleteUserGroupCommand> _deleteUserGroupCommandValidator;   
         public UserGroupController(ISender mediator,   
                    IValidator<CreateUserGroupCommand> createUserGroupCommandValidator, 
                    IValidator<UpdateUserGroupCommand> updateUserGroupCommandValidator, 
                    IValidator<DeleteUserGroupCommand> deleteUserGroupCommandValidator
                ) 
         : base(mediator)
        {        
            _createUserGroupCommandValidator = createUserGroupCommandValidator;    
            _updateUserGroupCommandValidator = updateUserGroupCommandValidator;   
            _deleteUserGroupCommandValidator = deleteUserGroupCommandValidator; 
        }
           [HttpGet]        
        public async Task<IActionResult> GetAllCountriesAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {   
            var countries = await Mediator.Send(
            new GetUserGroupQuery
           {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });

                  
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = countries.Message,
                data = countries.Data.ToList(),
                TotalCount = countries.TotalCount,
                PageNumber = countries.PageNumber,
                PageSize = countries.PageSize
            });
        }
        [HttpGet("{id}")]     
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Country ID"
                });
            }            
            var result = await Mediator.Send(new GetUserGroupByIdQuery { Id = id });            
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
        [HttpPost]          
        public async Task<IActionResult> CreateAsync(CreateUserGroupCommand  command)
        { 
            var validationResult = await _createUserGroupCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {   
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }                    
            var result = await Mediator.Send(command);
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
        [HttpPut]      
        public async Task<IActionResult> UpdateAsync( UpdateUserGroupCommand command)
        {
            var validationResult = await _updateUserGroupCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
            if (command.Id<=0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Country ID"
                });
            }        
            var result = await Mediator.Send(command);
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
        public async Task<IActionResult> DeleteAsync(int id)
        {
             var command = new DeleteUserGroupCommand { Id = id };
             var validationResult = await  _deleteUserGroupCommandValidator.ValidateAsync(command);
               if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        message = validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault(),
                        statusCode = StatusCodes.Status400BadRequest
                    });
                } 
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Country ID"
                });
            }            
              var result = await Mediator.Send(command);                 
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
                data =$"Country ID {id} Deleted" ,
                message = result.Message
            });
        }

        [HttpGet("by-name")]     
        public async Task<IActionResult> GetUserGroup([FromQuery] string? name)
        {
            var result = await Mediator.Send(new GetUserGroupAutoCompleteQuery { SearchPattern = name });
            if (!result.IsSuccess)
            {
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message,
                    data = result.Data
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