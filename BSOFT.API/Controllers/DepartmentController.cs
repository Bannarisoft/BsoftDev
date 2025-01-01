using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Departments.Queries.GetDepartmentById;
using Core.Application.Departments.Commands.CreateDepartment;
using Core.Application.Departments.Commands.UpdateDepartment;
using Core.Application.Departments.Commands.DeleteDepartment;
using Core.Application.Departments.Queries.GetDepartmentAutoComplete;
using Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch;
using Core.Application.Common.Interfaces;
using System.Data.Common;
using BSOFT.Infrastructure.Data;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ApiControllerBase
    {

          private readonly ApplicationDbContext _dbContext;
        public DepartmentController(ISender mediator ,ApplicationDbContext dbContext) : base(mediator)
        {
             _dbContext = dbContext; 
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
         [Route("Create")]
        public async Task<IActionResult>CreateAsync([FromBody] CreateDepartmentCommand command)
        {
            Console.WriteLine("DEPT Create");
            var createDepartment=await Mediator.Send(command);                    
             return Ok(new { Message = "Department created successfully.", id = createDepartment.Id });
               // return CreatedAtAction(nameof(GetByIdAsync),new {id=createdepartment.Id},createdepartment);
        }


      [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateDepartmentCommand command)
    {
       
        if (id != command.Id)
        {
            return BadRequest("UnitId Mismatch");
        }

        var UpdateDepartment = await Mediator.Send(command);
        return Ok("Updated Successfully");


    // Use the validator for UpdateCountryCommand
        // var validationResult = await _updateCountryCommandValidator.ValidateAsync(command);

        // if (!validationResult.IsValid)
        // {
        //     return BadRequest(validationResult.Errors);
        // }

        // if (countryid != command.Id)
        // {
        //     return BadRequest("CountryId Mismatch");
        // }   
        // var country = await _dbContext.Countries
        //                             .FirstOrDefaultAsync(c => c.Id == countryid && c.IsActive == 1);

        // if (country == null)
        // {
        //     return BadRequest("Only active countries (IsActive = 1) can be updated.");
        // }

        // await Mediator.Send(command);
        // return NoContent();



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
            var query = new GetDepartmentAutoCompleteSearchQuery { SearchPattern = searchDept };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

   


    }
}