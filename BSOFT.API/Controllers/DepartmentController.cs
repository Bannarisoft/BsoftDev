using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Departments.Queries.GetDepartmentById;
using Core.Application.Departments.Commands.CreateDepartment;
using Core.Application.Departments.Commands.UpdateDepartment;
using Core.Application.Departments.Commands.DeleteDepartment;
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

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = departments.Data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var  department = await Mediator.Send(new GetDepartmentByIdQuery() {DepartmentId=id});
            if(department ==null)
            {

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "Department Not Found"
                });               
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = department.Data
            });

        }
        [HttpPost]
         [Route("Create")]
        public async Task<IActionResult>CreateAsync([FromBody] CreateDepartmentCommand command)
        {

            var validationResult = await _createDepartmentCommandValidator.ValidateAsync(command);

              if (!validationResult.IsValid)
              {
              return BadRequest(new
              {
                  StatusCode = StatusCodes.Status400BadRequest,
                  message = "Validation failed",
                  errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
              });
              }
              var createdepartment = await Mediator.Send(command);
              if(createdepartment.IsSuccess)
              {
                  return Ok(new
                  {
                      StatusCode = StatusCodes.Status201Created,
                      message = createdepartment.Message,
                      data = createdepartment.Data
                  });
              }
              return BadRequest(new
              {
                  StatusCode = StatusCodes.Status400BadRequest,
                  message = createdepartment.Message
              });
               
        }




      [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync( UpdateDepartmentCommand command)
    {
         var department = await Mediator.Send(new  GetDepartmentByIdQuery() {DepartmentId=command.Id});
            if (department == null)
            {
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    Message = "Department not found" 
                });
            }

         var validationResult = await _updateDepartmentCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {

             return BadRequest(new
             {
                 StatusCode = StatusCodes.Status400BadRequest,
                 message = "Validation failed",
                 errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
             });
        }

       

        var UpdateDepartment = await Mediator.Send(command);
        if(UpdateDepartment.IsSuccess)
        {

            return Ok(new
            {
                message = UpdateDepartment.Message,
                statusCode = StatusCodes.Status200OK,
                data = UpdateDepartment.Data
            });
        }
        return BadRequest(new
        {
            message = UpdateDepartment.Message,
            statusCode = StatusCodes.Status400BadRequest
        });


    }

        
    [HttpPut("delete")]
        

        public async Task<IActionResult> Delete(DeleteDepartmentCommand deleteDepartmentCommand)
        {

            var department = await Mediator.Send(new  GetDepartmentByIdQuery() {DepartmentId=deleteDepartmentCommand.Id});
            if (department == null)
            {

                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    Message = "Department not found" 
                });
            }

           
          var result =  await Mediator.Send(deleteDepartmentCommand);


            if(result.IsSuccess)
            {
                return Ok(new
                {
                    message = result.Message,
                    statusCode = StatusCodes.Status200OK,
                    data = result.Data
                });
            }
            return BadRequest(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status400BadRequest
            });
          
        }



         [HttpGet("autocomplete")]
        public async Task<IActionResult> GetAllDepartmentAutoCompleteSearchAsync([FromQuery] string searchDept)
        {
            var query = new GetDepartmentAutoCompleteSearchQuery { SearchPattern = searchDept };
            var result = await Mediator.Send(query);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data
            });
        }

   


    }
    

   
}