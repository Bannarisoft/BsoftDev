using UserManagement.Infrastructure.Data;
using Core.Application.PasswordComplexityRule.Commands.UpdatePasswordComplexityRule;

//using Core.Application.PasswordComplexityRule.Commands.UpdatePasswordComplexityRule;
using Core.Application.PwdComplexityRule.Commands.CreatePasswordComplexityRule;
using Core.Application.PwdComplexityRule.Commands.DeletePasswordComplexityRule;
using Core.Application.PwdComplexityRule.Queries;
using Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRuleAutoComplete;
using Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRuleById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordComplexityRuleController :ApiControllerBase
    {
         private readonly IValidator<CreatePasswordComplexityRuleCommand> _createPasswordComplexityRuleCommand;
         private readonly IValidator<UpdatePasswordComplexityRuleCommand> _updatepasswordComplexityRuleCommandValidator; 
         private readonly ApplicationDbContext _dbContext;
         private readonly ILogger<PasswordComplexityRuleController> _logger;
         public PasswordComplexityRuleController(ISender mediator , 
         IValidator<CreatePasswordComplexityRuleCommand> createPasswordComplexityRuleCommandValidator, 
         IValidator<UpdatePasswordComplexityRuleCommand> updatePasswordComplexityRuleCommandValidator,
         ApplicationDbContext dbContext ,ILogger<PasswordComplexityRuleController> logger ) : base(mediator)
        {                      
             _createPasswordComplexityRuleCommand=createPasswordComplexityRuleCommandValidator;
             _updatepasswordComplexityRuleCommandValidator= updatePasswordComplexityRuleCommandValidator;
             _dbContext = dbContext; 
             _logger = logger;
        }

        [HttpGet]
       public async Task<IActionResult> GetPasswordComplexityAsync()
        {          
                _logger.LogInformation("Starting GetPasswordComplexityAsync request.");
                var pwdComplexityRules = await Mediator.Send(new GetPwdRuleQuery());

            if (pwdComplexityRules == null )
            {
                _logger.LogWarning("No password complexity rules found.");
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "No password complexity rules found."
                });
            }

            _logger.LogInformation("Password complexity rules retrieved successfully.");
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = pwdComplexityRules
            });

        
        }

         [HttpGet("{id}")]
       public async Task<IActionResult> GetByIdAsync(int id)
        {
           _logger.LogInformation("Starting GetByIdAsync for Password Complexity Rule with ID: {Id}", id);

            // Send the query to get the password complexity rule by ID
        var pwdComplexity = await Mediator.Send(new GetPwdComplexityRuleByIdQuery { Id = id });

        if (!pwdComplexity.IsSuccess )
        {
            _logger.LogWarning("Password Complexity Rule with ID: {Id} not found.", id);

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = pwdComplexity.Message
               
            });
        }

        _logger.LogInformation("Password Complexity Rule with ID: {Id} retrieved successfully.", id);

        return Ok(new
        {
            StatusCode = StatusCodes.Status200OK,
            Data = pwdComplexity
        });
          

        }
        
          [HttpGet("by-name/{name}")]
    public async Task<IActionResult> Getpwdautocomplete( string name)
    {
          _logger.LogInformation("Starting GetAllUserRoleAutoCompleteSearchAsync with search pattern: {SearchPattern}", name);
             var query = new GetPwdComplexityRuleAutoComplete { SearchTerm  = name };
                var result = await Mediator.Send(query);

                if (result.IsSuccess )
                {               

                     _logger.LogInformation("Password Complexity Rule found for search pattern: {SearchPattern}. Returning data.", name);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = result.Data
                });
                }
                   _logger.LogWarning("Password Complexity Rule found for search pattern: {SearchPattern}", name);

                  return NotFound(new
                 {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No matching Password Complexity Rule found.",

                });
  
    }

         [HttpPost]
        
        public async Task<IActionResult>CreateAsync([FromBody] CreatePasswordComplexityRuleCommand createPasswordComplexityRuleCommand)
        {
             _logger.LogInformation("Starting CreateAsync for creating a Password Complexity Rule.");

           // Validate the command
        var validationResult = await _createPasswordComplexityRuleCommand.ValidateAsync(createPasswordComplexityRuleCommand);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreatePasswordComplexityRuleCommand. Errors: {@Errors}", validationResult.Errors);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
            });
        }

        _logger.LogInformation("Validation passed for CreatePasswordComplexityRuleCommand. Proceeding with creation.");

        // Send the command to the Mediator
        var createPasswordComplexityRule = await Mediator.Send(createPasswordComplexityRuleCommand);
        if (createPasswordComplexityRule.IsSuccess)
            {
                _logger.LogInformation("Password Complexity Rule created successfully.");

                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Password Complexity Rule Created Successfully",
                    Data=createPasswordComplexityRule.Data
                
                });
          }
           _logger.LogWarning("Create Department request failed. Reason: {Message}", createPasswordComplexityRule.Message);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = createPasswordComplexityRule.Message

            });
             
        }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync( UpdatePasswordComplexityRuleCommand updatePasswordComplexityRuleCommand)
    {
          // Validate the command
        var validationResult = await _updatepasswordComplexityRuleCommandValidator.ValidateAsync(updatePasswordComplexityRuleCommand);
        if (!validationResult.IsValid)
        {          

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
            });
        }
      var PasswordComplexityRule = await Mediator.Send(new GetPwdComplexityRuleByIdQuery { Id = updatePasswordComplexityRuleCommand.Id });
        // Check for ID mismatch
        if (PasswordComplexityRule==null)
        {
            _logger.LogWarning($"Password Complexity Rule ID {updatePasswordComplexityRuleCommand.Id} mismatch." );

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "PasswordComplexityRule ID mismatch."
            });
        }

        _logger.LogInformation($"Validation passed. Proceeding to update Password Complexity Rule with ID: {updatePasswordComplexityRuleCommand.Id}");

        // Send the update command to the mediator
        var updateResult = await Mediator.Send(updatePasswordComplexityRuleCommand);

        if (updateResult == null)
        {
            _logger.LogWarning("Failed to update Password Complexity Rule with ID: {updatePasswordComplexityRuleCommand.Id}");

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to update Password Complexity Rule. Please try again later."
            });
        }

        _logger.LogInformation($"Password Complexity Rule with ID: {updatePasswordComplexityRuleCommand.Id} updated successfully.");

        return Ok(new
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Updated Successfully"
            
        });
    }

     [HttpDelete]
        
        public async Task<IActionResult> Delete(DeletePasswordComplexityRuleCommand  deletePasswordComplexityRuleCommand)
        {
             _logger.LogInformation($"Starting Delete operation for Password Complexity Rule with ID: {deletePasswordComplexityRuleCommand.Id}" );

         var PasswordComplexityRule = await Mediator.Send(new GetPwdComplexityRuleByIdQuery { Id = deletePasswordComplexityRuleCommand.Id });
          //var department = await Mediator.Send(new GetDepartmentByIdQuery { DepartmentId = deleteDepartmentCommand.Id });
                if (PasswordComplexityRule == null)
                {
                    _logger.LogWarning($"Password Complexity Rule with ID {deletePasswordComplexityRuleCommand.Id} not found.");

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Password Complexity Rule not found"
                    });
                }
          
                  var result = await Mediator.Send(deletePasswordComplexityRuleCommand);

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"PasswordComplexityRule with ID {deletePasswordComplexityRuleCommand.Id} deleted successfully.");

                    return Ok(new
                    {
                        Message = result.Message,
                        StatusCode = StatusCodes.Status200OK
                      
                    });
                }

            _logger.LogInformation($"Password Complexity Rule with ID {deletePasswordComplexityRuleCommand.Id} deleted successfully.");

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Password complexity rule with ID {deletePasswordComplexityRuleCommand.Id} deleted successfully."
            });         

        }

    }

}