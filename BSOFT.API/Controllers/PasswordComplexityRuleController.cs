using BSOFT.Infrastructure.Data;
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

namespace BSOFT.API.Controllers
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

        if (pwdComplexity == null)
        {
            _logger.LogWarning("Password Complexity Rule with ID: {Id} not found.", id);

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Password Complexity Rule not found."
            });
        }

        _logger.LogInformation("Password Complexity Rule with ID: {Id} retrieved successfully.", id);

        return Ok(new
        {
            StatusCode = StatusCodes.Status200OK,
            Data = pwdComplexity
        });
          

        }
        
         [HttpGet("PasswordRuleSearch")]
    public async Task<IActionResult> Getpwdautocomplete([FromQuery] string searchTerm)
    {
          _logger.LogInformation("Starting GetAllUserRoleAutoCompleteSearchAsync with search pattern: {SearchPattern}", searchTerm);
             var query = new GetPwdComplexityRuleAutoComplete { SearchTerm  = searchTerm };
                var result = await Mediator.Send(query);

                if (result.IsSuccess )
                {
                 

                     _logger.LogInformation("User Role found for search pattern: {SearchPattern}. Returning data.", searchTerm);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = result.Data
                });
                }
                   _logger.LogWarning("No User Role found for search pattern: {SearchPattern}", searchTerm);

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No matching User Role found."
                    });

               
     
  
    }

         [HttpPost]
         [Route("Create")]
        public async Task<IActionResult>CreateAsync([FromBody] CreatePasswordComplexityRuleCommand command)
        {
             _logger.LogInformation("Starting CreateAsync for creating a Password Complexity Rule.");

           // Validate the command
        var validationResult = await _createPasswordComplexityRuleCommand.ValidateAsync(command);
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
        var createPasswordComplexityRule = await Mediator.Send(command);

        _logger.LogInformation("Password Complexity Rule created successfully.");

        return Ok(new
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Created Successfully"
          
        });
             
        }

          [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdatePasswordComplexityRuleCommand command)
    {


        _logger.LogInformation("Starting UpdateAsync for Password Complexity Rule with ID: {Id}", id);
          // Validate the command
        var validationResult = await _updatepasswordComplexityRuleCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for UpdatePasswordComplexityRuleCommand. Errors: {@Errors}", validationResult.Errors);

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
            _logger.LogWarning("Password Complexity Rule ID mismatch. Route ID: {RouteId}, Command ID: {CommandId}", id, command.Id);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "PasswordComplexityRule ID mismatch."
            });
        }

        _logger.LogInformation("Validation passed. Proceeding to update Password Complexity Rule with ID: {Id}", id);

        // Send the update command to the mediator
        var updateResult = await Mediator.Send(command);

        if (updateResult == null)
        {
            _logger.LogWarning("Failed to update Password Complexity Rule with ID: {Id}", id);

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Failed to update Password Complexity Rule. Please try again later."
            });
        }

        _logger.LogInformation("Password Complexity Rule with ID: {Id} updated successfully.", id);

        return Ok(new
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Updated Successfully"
            
        });
    }

     [HttpPut("delete/{id}")]
        
        public async Task<IActionResult> Delete(int id,DeletePasswordComplexityRuleCommand deleteCommand)
        {
             _logger.LogInformation("Starting Delete operation for Password Complexity Rule with ID: {Id}", id);

        // Ensure the command ID matches the provided route ID
            if (id != deleteCommand.Id)
            {
                _logger.LogWarning("Password Complexity Rule ID mismatch. Route ID: {RouteId}, Command ID: {CommandId}", id, deleteCommand.Id);

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Password Complexity Rule ID mismatch."
                });
            }

            // Send the delete command to the mediator
            await Mediator.Send(deleteCommand);

            _logger.LogInformation("Password Complexity Rule with ID {Id} deleted successfully.", id);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Password complexity rule with ID {id} deleted successfully."
            });
       
  


        }





    }











    
}