using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BSOFT.Application.Departments.Queries.GetDepartments;
using BSOFT.Application.Departments.Queries.GetDepartmentById;
using BSOFT.Application.Departments.Commands.CreateDepartment;
using BSOFT.Application.Departments.Commands.UpdateDepartment;
using BSOFT.Application.Departments.Commands.DeleteDepartment;
using BSOFT.Domain.Interfaces;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ApiControllerBase
    {
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
            return CreatedAtAction(nameof(GetByIdAsync),new {id=createdepartment.DeptId},createdepartment);
        }


      [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateDepartmentCommand command)
    {
       
        if (id != command.DeptId)
        {
            return BadRequest("UnitId Mismatch");
        }

        var updateddepartment = await Mediator.Send(command);
        return Ok("Updated Successfully");
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await Mediator.Send(new DeleteDepartmentCommand { DeptId = id });
        if (result ==null)
        {
             return NotFound();
        }

        return Ok("Deleted Successfully");
    }


    }
}