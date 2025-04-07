using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.MiscMaster.Command.CreateMiscMaster;
using Core.Application.MiscMaster.Command.DeleteMiscMaster;
using Core.Application.MiscMaster.Command.UpdateMiscMaster;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using Core.Application.MiscMaster.Queries.GetMiscMasterAutoComplete;
using Core.Application.MiscMaster.Queries.GetMiscMasterById;
using Core.Domain.Entities;
using MaintenanceManagement.Infrastructure.Repositories.MiscMaster;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class MiscMasterController  : ApiControllerBase
    {
        private readonly ILogger<MiscMaster> _logger;
        private  readonly IValidator<CreateMiscMasterCommand> _miscMasterCommand;
        private readonly IValidator<UpdateMiscMasterCommand> _updateMiscMasterCommand;

        private readonly IValidator<DeleteMiscMasterCommand> _deleteMiscMasterCommand;
        private readonly IMiscMasterCommandRepository _miscMasterCommandRepository;
       public MiscMasterController(ISender mediator, IValidator<CreateMiscMasterCommand>  miscMasterCommand, IValidator<UpdateMiscMasterCommand> updateMiscMasterCommand, IMiscMasterCommandRepository miscMasterCommandRepository,
        IValidator<DeleteMiscMasterCommand> deleteMiscMasterCommand ):base(mediator)
          {

            _miscMasterCommand=miscMasterCommand;
            _updateMiscMasterCommand=updateMiscMasterCommand;
            _miscMasterCommandRepository=miscMasterCommandRepository;
            _deleteMiscMasterCommand = deleteMiscMasterCommand;
          } 

         [HttpGet] 
          public async Task<IActionResult> GetAllMiscMasterAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
            var miscmaster = await Mediator.Send(
            new GetMiscMasterQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           // var activecompanies = companies.Data.ToList(); 

            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = miscmaster.Data,
                TotalCount = miscmaster.TotalCount,
                PageNumber = miscmaster.PageNumber,
                PageSize = miscmaster.PageSize
            });
        }
        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
           
            var miscmaster = await Mediator.Send(new GetMiscMasterByIdQuery() { Id = id});
          
             if(miscmaster.IsSuccess)
            {
                 return Ok(new { StatusCode=StatusCodes.Status200OK, data = miscmaster.Data,message=miscmaster.Message});
            }

            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = $"MiscMaster ID {id} not found.", errors = "" });
           
        }
            [HttpGet("by-name")]
        public async Task<IActionResult> GetMiscMaster([FromQuery] string? name)
        {
          
            var miscmaster = await Mediator.Send(new GetMiscMasterAutoCompleteQuery {SearchPattern = name});
            if(miscmaster.IsSuccess)
            {
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = miscmaster.Data });
            }
            return NotFound( new { StatusCode=miscmaster.Message}) ;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateMiscMasterCommand command)
        {
             var validationResult = await _miscMasterCommand.ValidateAsync(command);
            
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
         public async Task<IActionResult> Update(UpdateMiscMasterCommand command)
        {
            // Update the record
              var validationResult = await _updateMiscMasterCommand.ValidateAsync(command);
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
        [ HttpDelete("{id}")]
          public async Task<IActionResult> Delete(int id)
        {
            
           var command = new DeleteMiscMasterCommand { Id = id };
           var validationResult = await  _deleteMiscMasterCommand.ValidateAsync(command);
               if (!validationResult.IsValid)
                {
                    return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
                }


           
           var updatedMiscMaster = await Mediator.Send(new DeleteMiscMasterCommand { Id = id });

           if(updatedMiscMaster.IsSuccess)
           {
            return Ok(new { StatusCode=StatusCodes.Status200OK, message = updatedMiscMaster.Message, errors = "" });
              
           }

            return BadRequest(new { StatusCode=StatusCodes.Status400BadRequest, message = updatedMiscMaster.Message, errors = "" });
            
        }
        
      
    }
}