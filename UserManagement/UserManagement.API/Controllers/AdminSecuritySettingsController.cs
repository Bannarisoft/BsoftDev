using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Application.AuditLog.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FluentValidation;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Commands.CreateAdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Commands.UpdateAdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Commands.DeleteAdminSecuritySettings;
using AutoMapper;
using Core.Domain.Entities;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettingsById;



namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminSecuritySettingsController : ApiControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<CreateAdminSecuritySettingsCommand> _createAdminSecuritySettingsCommandValidator;   
        private readonly IValidator <UpdateAdminSecuritySettingsCommand> _updateAdminSecuritysettingsCommandValidator;
        
        private readonly ILogger<AdminSecuritySettingsController> _logger;



        public AdminSecuritySettingsController (ISender mediator  , ApplicationDbContext dbContext,
        IValidator<CreateAdminSecuritySettingsCommand> createAdminSecuritySettingsCommandValidator, IValidator<UpdateAdminSecuritySettingsCommand> UpdateAdminSecuritySettingsCommandValidator,ILogger<AdminSecuritySettingsController> logger )  : base(mediator)
        {
             _dbContext = dbContext; 
             _createAdminSecuritySettingsCommandValidator = createAdminSecuritySettingsCommandValidator;
             _updateAdminSecuritysettingsCommandValidator = UpdateAdminSecuritySettingsCommandValidator;
             _logger = logger;

             
        }

         [HttpGet]
                public async Task<IActionResult> GetAllAdminSecuritySettingsAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            _logger.LogInformation("Fetching All Admin Security Settings Request started.");

            var adminSecuritySettings = await Mediator.Send(new GetAdminSecuritySettingsQuery
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchTerm = SearchTerm
            });

            if (adminSecuritySettings.Data == null || !adminSecuritySettings.Data.Any())
            {
                _logger.LogWarning("No admin security settings found in the database. Total count: {Count}", adminSecuritySettings?.Data?.Count ?? 0);

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = adminSecuritySettings.Message
                });
            }

            _logger.LogInformation("Admin security settings retrieved successfully.");

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = adminSecuritySettings.Data
            });
        }

    //    public async Task<IActionResult> GetAllAdminSecuritySettingsAsync( )
    //     {     
    //             _logger.LogInformation("Starting GetAllAdminSecuritySettingsAsync request.");
    //                     // Fetch the admin security settings
    //             var adminSecuritySettings = await Mediator.Send(new GetAdminSecuritySettingsQuery());

    //             if (adminSecuritySettings == null || !adminSecuritySettings.Data.Any())
    //             {
    //                 _logger.LogWarning($"No admin security settings found.{adminSecuritySettings.Data}");
    //                 return NotFound(new
    //                 {
    //                     StatusCode = StatusCodes.Status404NotFound,
    //                     Message = "No admin security settings found."
    //                 });
    //             }

    //             _logger.LogInformation("Admin security settings retrieved successfully.");

    //             return Ok(new
    //             {
    //                 StatusCode = StatusCodes.Status200OK,
    //                 Data = adminSecuritySettings
    //             });     
          
    //     }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
            {
                _logger.LogInformation($"Starting GetByIdAsync request for Admin Security Setting with ID: {id}");

                
                    // Fetch the admin security setting by ID
                    var adminSecuritySetting = await Mediator.Send(new GetAdminSecuritySettingsByIdQuery { Id = id });

                    if (adminSecuritySetting == null || adminSecuritySetting.Data == null)
                    {
                        _logger.LogWarning($"Admin Security Setting with ID {id} not found.");
                        return NotFound(new
                        {
                            StatusCode = StatusCodes.Status404NotFound,
                            Message = $"Admin Security Setting with ID {id} not found."
                        });
                    }

                    _logger.LogInformation($"Admin Security Setting with ID {id} retrieved successfully.");

                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Data = adminSecuritySetting
                    });
                
            
            }

            

         [HttpPost]        
        public async Task<IActionResult>CreateAsync([FromBody] CreateAdminSecuritySettingsCommand createAdminSecuritySettingscmd)
        {

               _logger.LogInformation($"Create AdminSecuritySettings request started with data: {createAdminSecuritySettingscmd}");

            // Validate the command
            var validationResult = await _createAdminSecuritySettingsCommandValidator.ValidateAsync(createAdminSecuritySettingscmd);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for Create AdminSecuritySettings request. Errors: {@Errors}", validationResult.Errors);

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            // Process the command
            var createAdminSecuritySettings = await Mediator.Send(createAdminSecuritySettingscmd);
            if (createAdminSecuritySettings.IsSuccess)
                {
                    _logger.LogInformation($"Create Department request succeeded. Department created with ID: {createAdminSecuritySettings.Data}");

                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status201Created,
                        Message = createAdminSecuritySettings.Message
                        
                    });
                }
            _logger.LogWarning($"Create Department request failed. Reason: {createAdminSecuritySettings.Message}");

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = createAdminSecuritySettings.Message
            });
            
        
        }
         
           [HttpPut]
            public async Task<IActionResult> UpdateAsync(int id, UpdateAdminSecuritySettingsCommand updateadminsecuritycommand)
            {
                 _logger.LogInformation($"Starting UpdateAsync for Admin Security Settings with ID: {id}.");
            // Validate the command
        _logger.LogDebug($"Validating UpdateAdminSecuritySettingsCommand: {updateadminsecuritycommand}.");
            var validationResult = await _updateAdminSecuritysettingsCommandValidator.ValidateAsync(updateadminsecuritycommand);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Validation failed for UpdateAdminSecuritySettingsCommand with ID: {id}. Errors: { validationResult.Errors}");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            // Check for ID mismatch
            if (id != updateadminsecuritycommand.Id)
            {
                _logger.LogWarning($"Admin Security Settings ID mismatch. URL ID: {id}, Command ID: {updateadminsecuritycommand.Id}" );
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Admin Security Settings ID mismatch"
                });
            }

            // Process the update command
            _logger.LogInformation("Sending UpdateAdminSecuritySettingsCommand for processing.");
            var updateResult = await Mediator.Send(updateadminsecuritycommand);

            if (updateResult.IsSuccess)
            {
                _logger.LogInformation($"Admin Security Settings with ID: {id} updated successfully.");
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Updated Successfully"
                    
                });
            }

            _logger.LogWarning($"Failed to update Admin Security Settings with ID: {id}. Reason: {updateResult.Message}");
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = updateResult.Message
            });
                
            }
            
          [HttpDelete("{id}")]       
        public async Task<IActionResult>Delete( int id)
        {
               // Check if the department exists
                var department = await Mediator.Send(new GetAdminSecuritySettingsByIdQuery { Id = id });
                if (department is null)
                {
                    _logger.LogWarning($"Admin Security Settings  with ID {id} not found." );

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Admin Security Settings  not found"
                    });
                }

                _logger.LogInformation($"Admin Security Settings  with ID {id} found. Proceeding with deletion.");

                // Attempt to delete the department
                var result = await Mediator.Send(new DeleteAdminSecuritySettingsCommand { Id = id });

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Admin Security Settings  with ID {id} deleted successfully.");

                    return Ok(new
                    {
                        Message = result.Message,
                        StatusCode = StatusCodes.Status200OK
                      
                    });
                }

                _logger.LogWarning($"Failed to delete Admin Security Settings  with ID {id}. Reason: {result.Message}"  );

                return BadRequest(new
                {
                    Message = result.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
          
        }


    }
}