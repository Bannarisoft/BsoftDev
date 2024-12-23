using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BSOFT.Application.Departments.Queries.GetDepartments;
using BSOFT.Application.Departments.Queries.GetDepartmentById;
using BSOFT.Application.Departments.Commands.CreateDepartment;
using BSOFT.Application.Departments.Commands.UpdateDepartment;
using BSOFT.Application.Departments.Commands.DeleteDepartment;
using BSOFT.Application.Departments.Queries.GetDepartmentAutoComplete;
using BSOFT.Application.Departments.Queries.GetDepartmentAutoCompleteSearch;
using BSOFT.Application.Common.Interfaces;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ApiControllerBase
    {
        public DepartmentController(ISender mediator) : base(mediator)
        {
        }
       [HttpGet]
       public async Task<IActionResult> GetAllDepartmentAsync()
        {          
            var departments =await Mediator.Send(new GetDepartmentQuery());
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var  department = await Mediator.Send(new GetDepartmentByIdQuery() {DepartmentId=id});
            if(department ==null)
            {
                return  NotFound();                
            }
            return Ok(department);

        }
        [HttpPost]
        public async Task<IActionResult>CreateAsync(CreateDepartmentCommand command)
        {
            var createdepartment=await Mediator.Send(command);
            return Ok("Created successfully");
            return CreatedAtAction(nameof(GetByIdAsync),new {id=createdepartment.Id},createdepartment);
        }


      [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateDepartmentCommand command)
    {
       
        if (id != command.Id)
        {
            return BadRequest("UnitId Mismatch");
        }

        var updateddepartment = await Mediator.Send(command);
        return Ok("Updated Successfully");
    }

    [HttpPut("delete/{id}")]
        
        public async Task<IActionResult> Delete(int id,DeleteDepartmentCommand deleteDepartmentCommand)
        {
             if(id != deleteDepartmentCommand.Id)
            {
                return BadRequest();
            }
            await Mediator.Send(deleteDepartmentCommand);

            return NoContent();
        }

        [HttpGet("GetAutoComplete")]
       public async Task<IActionResult> GetAllDepartmentAutoCompleteAsync()
        {          
            var departments =await Mediator.Send(new GetDepartmentAutoCompleteQuery());
         
            return Ok(departments);
        }

         [HttpGet("autocomplete")]
        public async Task<IActionResult> GetAllDepartmentAutoCompleteSearchAsync([FromQuery] string searchDept)
        {
            var query = new GetDepartmentAutoCompleteSearchQuery { SearchDept = searchDept };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

   


    }
}