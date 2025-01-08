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
using FluentValidation;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ApiControllerBase
    {

        private readonly IValidator<CreateDepartmentCommand> _createDepartmentCommandValidator;
        private readonly IValidator<UpdateDepartmentCommand> _updateDepartmentCommandValidator;
          private readonly ApplicationDbContext _dbContext;
        public DepartmentController(ISender mediator , IValidator<CreateDepartmentCommand> createDepartmentCommandValidator
        ,IValidator<UpdateDepartmentCommand> updateDepartmentCommandValidator, ApplicationDbContext dbContext  ) : base(mediator)
        {
            _createDepartmentCommandValidator=createDepartmentCommandValidator;
            _updateDepartmentCommandValidator=updateDepartmentCommandValidator;
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
                BadRequest("ID in the URL does not match the command Department.");               
            }
            return Ok(department);

        }
        [HttpPost]
         [Route("Create")]
        public async Task<IActionResult>CreateAsync([FromBody] CreateDepartmentCommand command)
        {

            var validationResult = await _createDepartmentCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
        var createdepartment = await Mediator.Send(command);
        return Ok("Created Successfully");         
        }


      [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateDepartmentCommand command)
    {
         var validationResult = await _updateDepartmentCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
        if (id != command.Id)
        {
            return BadRequest("Department Id Mismatch");
        }

        var UpdateDepartment = await Mediator.Send(command);
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
            var query = new GetDepartmentAutoCompleteSearchQuery { SearchPattern = searchDept };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

   


    }
   
}