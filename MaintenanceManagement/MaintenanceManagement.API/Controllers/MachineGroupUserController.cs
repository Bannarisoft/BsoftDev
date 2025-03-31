
using Core.Application.MachineGroupUser.Command.DeleteMachineGroupUser;
using Core.Application.MachineGroupUser.Command.UpdateMachineGroupUser;
using Core.Application.MachineGroupUser.Queries.GetMachineGroupUser;
using Core.Application.MachineGroupUser.Queries.GetMachineGroupUserAutoComplete;
using Core.Application.MachineGroupUser.Queries.GetMachineGroupUserById;
using Core.Application.MachineGroupUsers.Command.CreateMachineGroupUser;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineGroupUserController  : ApiControllerBase
    {
        private readonly IValidator<CreateMachineGroupUserCommand> _createMachineGroupUserCommandValidator;
        private readonly IValidator<UpdateMachineGroupUserCommand> _updateMachineGroupUserCommandValidator;
        private readonly IValidator<DeleteMachineGroupUserCommand> _deleteMachineGroupUserCommandValidator;
        
        public MachineGroupUserController(ISender mediator, IValidator<CreateMachineGroupUserCommand> createMachineGroupUserCommandValidator, IValidator<UpdateMachineGroupUserCommand> updateMachineGroupUserCommandValidator, IValidator<DeleteMachineGroupUserCommand> deleteMachineGroupUserCommandValidator)
        : base(mediator)
        {
            _createMachineGroupUserCommandValidator = createMachineGroupUserCommandValidator;
            _updateMachineGroupUserCommandValidator = updateMachineGroupUserCommandValidator;
            _deleteMachineGroupUserCommandValidator = deleteMachineGroupUserCommandValidator;
        }
           [HttpGet]
        public async Task<IActionResult> GetAllShiftMastersAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var divisions = await Mediator.Send(
            new GetMachineGroupUserQuery
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
        public async Task<IActionResult> CreateAsync(CreateMachineGroupUserCommand command)
        {
            
            var validationResult = await _createMachineGroupUserCommandValidator.ValidateAsync(command);
            
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
               
            var shiftMaster = await Mediator.Send(new GetMachineGroupUserByIdQuery { Id = id});
          
             if(shiftMaster == null)
            {
                return NotFound( new 
                { 
                    StatusCode=StatusCodes.Status404NotFound, 
                    message = $"Shift Master ID {id} not found.", 
                    errors = "" 
                });
            }
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = shiftMaster.Data
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update( UpdateMachineGroupUserCommand command )
        {
            var validationResult = await _updateMachineGroupUserCommandValidator.ValidateAsync(command);
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
            var command = new DeleteMachineGroupUserCommand { Id = id };
             var validationResult = await  _deleteMachineGroupUserCommandValidator.ValidateAsync(command);
               if (!validationResult.IsValid)
                {
                    return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
                }
           var updatedShiftMaster = await Mediator.Send(command);

           if(updatedShiftMaster.IsSuccess)
           {
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = updatedShiftMaster.Message, 
                errors = "" 
            });
              
           }

            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = updatedShiftMaster.Message, 
                errors = "" 
            });
            
        }

         [HttpGet("by-name")]
        public async Task<IActionResult> GetShiftMaster([FromQuery] string? name)
        {
           
           var shiftMaster = await Mediator.Send(new GetMachineGroupUserAutoCompleteQuery {SearchPattern = name});
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = shiftMaster.Data 
            });
        }
    }
}