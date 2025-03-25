using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Application.MachineGroup.Command.CreateMachineGroup;
using Core.Application.MachineGroup.Command.DeleteMachineGroup;
using Core.Application.MachineGroup.Command.UpdateMachineGroup;
using Core.Application.MachineGroup.Quries.GetMachineGroup;
using Core.Application.MachineGroup.Quries.GetMachineGroupAutoComplete;
using Core.Application.MachineGroup.Quries.GetMachineGroupById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class MachineGroupController : ApiControllerBase
    {
        private readonly IMachineGroupCommandRepository _machineGroupCommandRepository;

         private  readonly IValidator<CreateMachineGroupCommand>  _createMachineGroupCommandValidator;
         private readonly IValidator<UpdateMachineGroupCommand> _updateMachineGroupCommandValidator;

        

        public MachineGroupController(ISender mediator, IMachineGroupCommandRepository machineGroupCommandRepository,IValidator<CreateMachineGroupCommand>  createMachineGroupCommandValidator, IValidator<UpdateMachineGroupCommand> updateMachineGroupCommandValidator   ):base(mediator)
        {
            _machineGroupCommandRepository = machineGroupCommandRepository;
            _createMachineGroupCommandValidator = createMachineGroupCommandValidator;
            _updateMachineGroupCommandValidator = updateMachineGroupCommandValidator;
          
        }

        [HttpGet] 
          public async Task<IActionResult> GetAllMachineGroupsAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
            var machinegroup = await Mediator.Send(
            new GetMachineGroupQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           // var activecompanies = companies.Data.ToList(); 

            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = machinegroup.Data,
                TotalCount = machinegroup.TotalCount,
                PageNumber = machinegroup.PageNumber,
                PageSize = machinegroup.PageSize
            });
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
           
            var machinegroup = await Mediator.Send(new GetMachineGroupByIdQuery() { Id = id});
          
             if(machinegroup.IsSuccess)
            {
                 return Ok(new { StatusCode=StatusCodes.Status200OK, data = machinegroup.Data,message=machinegroup.Message});
            }

            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = $"MachineGroup ID {id} not found.", errors = "" });
           
        }


         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateMachineGroupCommand command)
        {
             var validationResult = await _createMachineGroupCommandValidator.ValidateAsync(command);
            
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
        public async Task<IActionResult> UpdateAsync(UpdateMachineGroupCommand command)
        {         
            var validationResult = await _updateMachineGroupCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(
                    new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        message = "Validation failed",
                        errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                    }
                );
            }            
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new 
                {   StatusCode=StatusCodes.Status200OK,
                    message = result.Message, 
                    asset = result.Data
                });
            }
                
                return BadRequest( new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = result.Message
                });
                
        }

       [ HttpDelete("{id}")]
          public async Task<IActionResult> Delete(int id)
        {
           
           var updatedMiscMaster = await Mediator.Send(new DeleteMachineGroupCommand { Id = id });

           if(updatedMiscMaster.IsSuccess)
           {
            return Ok(new { StatusCode=StatusCodes.Status200OK, message = updatedMiscMaster.Message, errors = "" });
              
           }

            return BadRequest(new { StatusCode=StatusCodes.Status400BadRequest, message = updatedMiscMaster.Message, errors = "" });
            
        }
         [HttpGet("by-name")]
        public async Task<IActionResult> GetMachineGroup([FromQuery] string? name)
        {
          
            var miscmaster = await Mediator.Send(new GetMiscMasterAutoCompleteQuery {SearchPattern = name});
            if(miscmaster.IsSuccess)
            {
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = miscmaster.Data });
            }
            return NotFound( new { StatusCode=miscmaster.Message}) ;
        }
        



    }
}