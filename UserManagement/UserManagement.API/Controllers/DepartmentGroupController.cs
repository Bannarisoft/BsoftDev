using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.DepartmentGroup.Command.CreateDepartmentGroup;
using Core.Application.DepartmentGroup.Command.DeleteDepartmentGroup;
using Core.Application.DepartmentGroup.Command.UpdateDepartmentGroup;
using Core.Application.DepartmentGroup.Queries.GetAllDepartmentGroup;
using Core.Application.DepartmentGroup.Queries.GetDepartmentGroupAutoSearch;
using Core.Application.DepartmentGroup.Queries.GetDepartmentGroupById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagement.Infrastructure.Data;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentGroupController : ApiControllerBase
    {
        private readonly IValidator<CreateDepartmentGroupCommand> _createDepartmentGroupCommandValidator;
        private readonly IValidator<DeleteDepartmentGroupCommand> _deleteDepartmentGroupCommandValidator;
        private readonly IValidator<UpdateDepartmentGroupCommand> _updateDepartmentGroupCommandValidator;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DepartmentController> _logger;


        public DepartmentGroupController(ISender mediator, IValidator<CreateDepartmentGroupCommand> createDepartmentGroupCommandValidator, ApplicationDbContext dbContext, ILogger<DepartmentController> logger,
            IValidator<DeleteDepartmentGroupCommand> deleteDepartmentGroupCommandValidator, IValidator<UpdateDepartmentGroupCommand> updateDepartmentGroupCommandValidator) : base(mediator)
        {
            _createDepartmentGroupCommandValidator = createDepartmentGroupCommandValidator;
            _dbContext = dbContext;
            _logger = logger;
            _updateDepartmentGroupCommandValidator = updateDepartmentGroupCommandValidator;
            _deleteDepartmentGroupCommandValidator = deleteDepartmentGroupCommandValidator;

        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateDepartmentGroupCommand command)
        {


            // Validate the command
            var validationResult = await _createDepartmentGroupCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {


                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            // Process the command
            var createdepartmentgroup = await Mediator.Send(command);
            if (createdepartmentgroup.IsSuccess)
            {


                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = createdepartmentgroup.Message,
                    Data = createdepartmentgroup.Data
                });
            }
            else
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = createdepartmentgroup.Message,
                    Data = createdepartmentgroup.Data
                });
            }
        }

        [HttpGet("GetAllDepartmentGroup")]
        public async Task<IActionResult> GetAllDepartmentGroupAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {


            var departmentGroups = await Mediator.Send(
                new GetAllDepartmentGroupQuery
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm
                });

            if (departmentGroups.Data == null || !departmentGroups.Data.Any())
            {


                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = departmentGroups.Message
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = departmentGroups.Data,
                totalCount = departmentGroups.TotalCount,
                pageNumber = departmentGroups.PageNumber,
                pageSize = departmentGroups.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await Mediator.Send(new GetDepartmentGroupByIdQuery { Id = id });

            if (result == null || result.Data == null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = result?.Message ?? "Department Group not found."
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = result.Data
            });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateDepartmentGroupCommand command)
        {


            var validationResult = await _updateDepartmentGroupCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
            var result = await Mediator.Send(command);

            if (!result.IsSuccess || result.Data == 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = result.Message
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = result.Message,
                Data = result.Data
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteDepartmentGroupCommand { Id = id };

            // Validate the delete command
            var validationResult = await _deleteDepartmentGroupCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            // Check if the department group exists
            var department = await Mediator.Send(new GetDepartmentGroupByIdQuery { Id = id });
            if (department == null || department.Data == null)
            {

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Department Group ID {id} not found"
                });
            }



            // Attempt deletion
            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = result.Message
                });
            }

            _logger.LogWarning("Failed to delete Department Group with ID {Id}. Reason: {Reason}", id, result.Message);
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = result.Message
            });
        }
            

      [HttpGet("by-name")]  
        public async Task<IActionResult> GetAllDepartmentGroupAutocompleteAsync([FromQuery] string? name)
        {           
            var result = await Mediator.Send(new GetDepartmentGroupAutoCompleteQuery {SearchPattern = name}); // Pass `searchPattern` to the constructor
            if (result.IsSuccess)
            {
                return Ok(new 
                {
                    StatusCode=StatusCodes.Status200OK,
                    message = result.Message,
                    data = result.Data
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = result.Message,
                data = result.Data
            });
        }  



      
    }
}