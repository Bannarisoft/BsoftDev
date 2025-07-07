using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Application.MachineGroup.Command.CreateMachineGroup;
using Core.Application.MachineGroup.Command.DeleteMachineGroup;
using Core.Application.MachineGroup.Command.UpdateMachineGroup;
using Core.Application.MachineGroup.Queries.GetMachineGroup;
using Core.Application.MachineGroup.Queries.GetMachineGroupAutoComplete;
using Core.Application.MachineGroup.Queries.GetMachineGroupById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class MachineGroupController : ApiControllerBase
    {
          

        

        public MachineGroupController(ISender mediator   ):base(mediator)
        {
            
          
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
          
            return Ok(new { StatusCode=StatusCodes.Status200OK, data = machinegroup,message=machinegroup});
           
        }


         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateMachineGroupCommand command)
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
        public async Task<IActionResult> UpdateAsync(UpdateMachineGroupCommand command)
        {         
          
            var result = await Mediator.Send(command);
           
                return Ok(new 
                {   StatusCode=StatusCodes.Status200OK,
                    message = result, 
                    asset = result
                });
            
                
        }

       [ HttpDelete("{id}")]
          public async Task<IActionResult> Delete(int id)
        {
           
           var updatedMiscMaster = await Mediator.Send(new DeleteMachineGroupCommand { Id = id });

            return Ok(new { StatusCode=StatusCodes.Status200OK, message = updatedMiscMaster, errors = "" });
              
            
        }
         [HttpGet("by-name")]
        public async Task<IActionResult> GetMachineGroup([FromQuery] string? name)
        {
          
            var miscmaster = await Mediator.Send(new GetMachineGroupAutoCompleteQuery {SearchPattern = name});
          
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = miscmaster });
            
        }
        



    }
}