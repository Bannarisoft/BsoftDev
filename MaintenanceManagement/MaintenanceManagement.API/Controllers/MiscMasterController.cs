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
        
        
       public MiscMasterController(ISender mediator, IValidator<CreateMiscMasterCommand> miscMasterCommand, IValidator<UpdateMiscMasterCommand> updateMiscMasterCommand, IMiscMasterCommandRepository miscMasterCommandRepository,
          IValidator<DeleteMiscMasterCommand> deleteMiscMasterCommand) : base(mediator)
        {

            _miscMasterCommand = miscMasterCommand;
            _updateMiscMasterCommand = updateMiscMasterCommand;
            _miscMasterCommandRepository = miscMasterCommandRepository;
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
          
            return Ok(new { StatusCode=StatusCodes.Status200OK, data = miscmaster,message=miscmaster});
        
           
        }
            [HttpGet("by-name")]
        public async Task<IActionResult> GetMiscMaster([FromQuery] string? name,[FromQuery] string MiscTypeCode)
        {
          
            var miscmaster = await Mediator.Send(new GetMiscMasterAutoCompleteQuery {SearchPattern = name,MiscTypeCode=MiscTypeCode});
            
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = miscmaster });
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateMiscMasterCommand command)
        {
           
          
            var response = await Mediator.Send(command);
                                      
                return Ok(new 
                {
                     StatusCode=StatusCodes.Status201Created,
                 message = response,
                  errors = "",
                  data = response 
                  });
            
            
        } 
        [HttpPut] 
         public async Task<IActionResult> Update(UpdateMiscMasterCommand command)
        {
           

            var response = await Mediator.Send(command);
            
                return Ok(new 
                { 
                    StatusCode = StatusCodes.Status200OK, 
                    Message = response, 
                    Errors = "" 
                });
            

        }
        [ HttpDelete("{id}")]
          public async Task<IActionResult> Delete(int id)
        {
        
           
           var updatedMiscMaster = await Mediator.Send(new DeleteMiscMasterCommand { Id = id });

            return Ok(new { StatusCode=StatusCodes.Status200OK, message = updatedMiscMaster, errors = "" });
              
            
        }
        
    }
}