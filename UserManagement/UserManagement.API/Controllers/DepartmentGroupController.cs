using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.DepartmentGroup.Command.CreateDepartmentGroup;
using Core.Application.DepartmentGroup.Command.UpdateDepartmentGroup;
using Core.Application.DepartmentGroup.Queries.GetAllDepartmentGroup;
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
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DepartmentController> _logger;


        public DepartmentGroupController(ISender mediator, IValidator<CreateDepartmentGroupCommand> createDepartmentGroupCommandValidator, ApplicationDbContext dbContext, ILogger<DepartmentController> logger) : base(mediator)
        {
            _createDepartmentGroupCommandValidator = createDepartmentGroupCommandValidator;
            _dbContext = dbContext;
            _logger = logger;
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


      
    }
}