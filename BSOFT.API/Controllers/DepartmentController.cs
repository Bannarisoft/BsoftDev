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
using Microsoft.Extensions.Logging;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class DepartmentController : ApiControllerBase
    {

        private readonly IValidator<CreateDepartmentCommand> _createDepartmentCommandValidator;
        private readonly IValidator<UpdateDepartmentCommand> _updateDepartmentCommandValidator;
          private readonly ApplicationDbContext _dbContext;
          private readonly ILogger<DepartmentController> _logger;
        public DepartmentController( ISender mediator , IValidator<CreateDepartmentCommand> createDepartmentCommandValidator
        ,IValidator<UpdateDepartmentCommand> updateDepartmentCommandValidator, ApplicationDbContext dbContext ,ILogger<DepartmentController> logger ) : base(mediator)
        {
            _createDepartmentCommandValidator=createDepartmentCommandValidator;
            _updateDepartmentCommandValidator=updateDepartmentCommandValidator;
             _dbContext = dbContext; 
             _logger = logger;

        }
       [HttpGet]
       public async Task<IActionResult> GetAllDepartmentAsync()
        {       
           _logger.LogInformation("Fetching All Department Request started.");
           
            var departments =await Mediator.Send(new GetDepartmentQuery());
           if (departments.Data == null || !departments.Data.Any())
            {
               
                _logger.LogInformation("No department records found in the database. Total count: {Count}", departments?.Data?.Count ?? 0);
                 return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        message = departments.Message
                    });
             }           
          
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = departments.Data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching single department with ID {Id} request started.", id);

            // Retrieve the department
            var department = await Mediator.Send(new GetDepartmentByIdQuery { DepartmentId = id });

            // Check if the department exists
            if (department == null || department.Data == null)
            {
                _logger.LogInformation("Department with ID {Id} not found in the database.", id);

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = department?.Message ?? "Department not found."
                });
            }

            // Return success response
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = department.Data
            });
         
        }
        [HttpPost]
         [Route("Create")]
        public async Task<IActionResult>CreateAsync([FromBody] CreateDepartmentCommand command)
        {
                _logger.LogInformation("Create Department request started with data: {@Command}", command);

            // Validate the command
            var validationResult = await _createDepartmentCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for Create Department request. Errors: {@Errors}", validationResult.Errors);

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            // Process the command
            var createdepartment = await Mediator.Send(command);
            if (createdepartment.IsSuccess)
            {
                _logger.LogInformation("Create Department request succeeded. Department created with ID: {DepartmentId}", createdepartment.Data.Id);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = createdepartment.Message,
                    Data = createdepartment.Data
                });
            }
            _logger.LogWarning("Create Department request failed. Reason: {Message}", createdepartment.Message);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = createdepartment.Message
            });
            
               
        }


      [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync( UpdateDepartmentCommand command)
    {
                _logger.LogInformation("Update Department request started with data: {@Command}", command);

            // Check if the department exists
            var department = await Mediator.Send(new GetDepartmentByIdQuery { DepartmentId = command.Id });
            if (department == null)
            {
                _logger.LogWarning("Department with ID {DepartmentId} not found.", command.Id);

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Department not found"
                });
            }

            // Validate the update command
            var validationResult = await _updateDepartmentCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for Update Department request. Errors: {@Errors}", validationResult.Errors);

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

          

            // Update the department
            var updateResult = await Mediator.Send(command);
            if (updateResult.IsSuccess)
            {
                _logger.LogInformation("Department with ID {DepartmentId} updated successfully.", command.Id);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Department updated successfully"
                  
                });
            }

            _logger.LogWarning("Failed to update Department with ID {DepartmentId}. Reason: {Message}", command.Id, updateResult.Message);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = updateResult.Message
            });
        
    }

        
    [HttpPut("delete")]
        

        public async Task<IActionResult> Delete(DeleteDepartmentCommand deleteDepartmentCommand)
        {
            _logger.LogInformation("Delete Department request started with ID: {DepartmentId}", deleteDepartmentCommand.Id);

                // Check if the department exists
                var department = await Mediator.Send(new GetDepartmentByIdQuery { DepartmentId = deleteDepartmentCommand.Id });
                if (department == null)
                {
                    _logger.LogWarning("Department with ID {DepartmentId} not found.", deleteDepartmentCommand.Id);

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Department not found"
                    });
                }

                _logger.LogInformation("Department with ID {DepartmentId} found. Proceeding with deletion.", deleteDepartmentCommand.Id);

                // Attempt to delete the department
                var result = await Mediator.Send(deleteDepartmentCommand);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Department with ID {DepartmentId} deleted successfully.", deleteDepartmentCommand.Id);

                    return Ok(new
                    {
                        Message = result.Message,
                        StatusCode = StatusCodes.Status200OK
                      
                    });
                }

                _logger.LogWarning("Failed to delete Department with ID {DepartmentId}. Reason: {Message}", deleteDepartmentCommand.Id, result.Message);

                return BadRequest(new
                {
                    Message = result.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });


     
        }


         [HttpGet("autocomplete")]
        public async Task<IActionResult> GetAllDepartmentAutoCompleteSearchAsync([FromQuery] string searchDept)
        {
            _logger.LogInformation("Starting GetAllDepartmentAutoCompleteSearchAsync with search pattern: {SearchPattern}", searchDept);
             var query = new GetDepartmentAutoCompleteSearchQuery { SearchPattern = searchDept };
                var result = await Mediator.Send(query);

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No departments found for search pattern: {SearchPattern}", searchDept);

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No matching departments found."
                    });
                }

                _logger.LogInformation("Departments found for search pattern: {SearchPattern}. Returning data.", searchDept);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = result.Data
                });
            // var query = new GetDepartmentAutoCompleteSearchQuery { SearchPattern = searchDept };
            // var result = await Mediator.Send(query);

            // return Ok(new
            // {
            //     StatusCode = StatusCodes.Status200OK,
            //     data = result.Data
            // });
        }

   


    }
    

   
}