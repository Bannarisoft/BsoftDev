using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
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



namespace BSOFT.API.Controllers
{
    [Route("[controller]")]
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
       public async Task<IActionResult> GetAllAdminSecuritySettingsAsync( )
        {     
                _logger.LogInformation("Starting GetAllAdminSecuritySettingsAsync request.");
                        // Fetch the admin security settings
                var adminSecuritySettings = await Mediator.Send(new GetAdminSecuritySettingsQuery());

                if (adminSecuritySettings == null || !adminSecuritySettings.Data.Any())
                {
                    _logger.LogWarning("No admin security settings found.");
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No admin security settings found."
                    });
                }

                _logger.LogInformation("Admin security settings retrieved successfully.");

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = adminSecuritySettings
                });     
           // var AdminSecuritySettings =await Mediator.Send(new GetAdminSecuritySettingsQuery());
            //return Ok(AdminSecuritySettings);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
            {
                _logger.LogInformation("Starting GetByIdAsync request for Admin Security Setting with ID: {Id}", id);

                
                    // Fetch the admin security setting by ID
                    var adminSecuritySetting = await Mediator.Send(new GetAdminSecuritySettingsByIdQuery { Id = id });

                    if (adminSecuritySetting == null || adminSecuritySetting.Data == null)
                    {
                        _logger.LogWarning("Admin Security Setting with ID {Id} not found.", id);
                        return NotFound(new
                        {
                            StatusCode = StatusCodes.Status404NotFound,
                            Message = $"Admin Security Setting with ID {id} not found."
                        });
                    }

                    _logger.LogInformation("Admin Security Setting with ID {Id} retrieved successfully.", id);

                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Data = adminSecuritySetting
                    });
                
            
            }

         [HttpPost]
         [Route("Create")]
        public async Task<IActionResult>CreateAsync([FromBody] CreateAdminSecuritySettingsCommand command)
        {

               _logger.LogInformation("Create AdminSecuritySettings request started with data: {@Command}", command);

            // Validate the command
            var validationResult = await _createAdminSecuritySettingsCommandValidator.ValidateAsync(command);
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
            var createAdminSecuritySettings = await Mediator.Send(command);
            if (createAdminSecuritySettings.IsSuccess)
                {
                    _logger.LogInformation("Create Department request succeeded. Department created with ID: {DepartmentId}", createAdminSecuritySettings.Data.Id);

                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status201Created,
                        Message = createAdminSecuritySettings.Message
                        
                    });
                }
            _logger.LogWarning("Create Department request failed. Reason: {Message}", createAdminSecuritySettings.Data);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = createAdminSecuritySettings.Message
            });
            
        
        }
         
           [HttpPut("update/{id}")]
            public async Task<IActionResult> UpdateAsync(int id, UpdateAdminSecuritySettingsCommand command)
            {
                 _logger.LogInformation("Starting UpdateAsync for Admin Security Settings with ID: {Id}.", id);
 // Validate the command
        _logger.LogDebug("Validating UpdateAdminSecuritySettingsCommand: {@Command}.", command);
        var validationResult = await _updateAdminSecuritysettingsCommandValidator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for UpdateAdminSecuritySettingsCommand with ID: {Id}. Errors: {@Errors}", id, validationResult.Errors);
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
            });
        }

        // Check for ID mismatch
        if (id != command.Id)
        {
            _logger.LogWarning("Admin Security Settings ID mismatch. URL ID: {UrlId}, Command ID: {CommandId}", id, command.Id);
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Admin Security Settings ID mismatch"
            });
        }

        // Process the update command
        _logger.LogInformation("Sending UpdateAdminSecuritySettingsCommand for processing.");
        var updateResult = await Mediator.Send(command);

        if (updateResult.IsSuccess)
        {
            _logger.LogInformation("Admin Security Settings with ID: {Id} updated successfully.", id);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Updated Successfully"
                
            });
        }

        _logger.LogWarning("Failed to update Admin Security Settings with ID: {Id}. Reason: {Message}", id, updateResult.Message);
        return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = updateResult.Message
        });
              
            }
            
             [HttpPut("delete/{id}")]        
        public async Task<IActionResult>Delete(int id, DeleteAdminSecuritySettingsCommand deleteAdminSecuritySettingCommand)
        {
             _logger.LogInformation("Delete Admin Security Settings  request started with ID: {Id}", deleteAdminSecuritySettingCommand.Id);

                // Check if the department exists
                var department = await Mediator.Send(new GetAdminSecuritySettingsByIdQuery { Id = deleteAdminSecuritySettingCommand.Id });
                if (department == null)
                {
                    _logger.LogWarning("Admin Security Settings  with ID {Id} not found.", deleteAdminSecuritySettingCommand.Id);

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Admin Security Settings  not found"
                    });
                }

                _logger.LogInformation("Admin Security Settings  with ID {Id} found. Proceeding with deletion.", deleteAdminSecuritySettingCommand.Id);

                // Attempt to delete the department
                var result = await Mediator.Send(deleteAdminSecuritySettingCommand);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Admin Security Settings  with ID {Id} deleted successfully.", deleteAdminSecuritySettingCommand.Id);

                    return Ok(new
                    {
                        Message = result.Message,
                        StatusCode = StatusCodes.Status200OK
                      
                    });
                }

                _logger.LogWarning("Failed to delete Admin Security Settings  with ID {d}. Reason: {Message}", deleteAdminSecuritySettingCommand.Id, result.Message);

                return BadRequest(new
                {
                    Message = result.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
          
        }


    }
}