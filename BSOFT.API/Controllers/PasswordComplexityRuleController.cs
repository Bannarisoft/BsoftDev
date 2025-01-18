using BSOFT.Infrastructure.Data;
using Core.Application.PasswordComplexityRule.Commands.UpdatePasswordComplexityRule;

//using Core.Application.PasswordComplexityRule.Commands.UpdatePasswordComplexityRule;
using Core.Application.PwdComplexityRule.Commands.CreatePasswordComplexityRule;
using Core.Application.PwdComplexityRule.Commands.DeletePasswordComplexityRule;
using Core.Application.PwdComplexityRule.Queries;
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
         public PasswordComplexityRuleController(ISender mediator , 
         IValidator<CreatePasswordComplexityRuleCommand> createPasswordComplexityRuleCommandValidator, 
         IValidator<UpdatePasswordComplexityRuleCommand> updatePasswordComplexityRuleCommandValidator,
         ApplicationDbContext dbContext  ) : base(mediator)
        {                      
             _createPasswordComplexityRuleCommand=createPasswordComplexityRuleCommandValidator;
             _updatepasswordComplexityRuleCommandValidator= updatePasswordComplexityRuleCommandValidator;
             _dbContext = dbContext; 
        }

        [HttpGet]
       public async Task<IActionResult> GetPasswordComplexityAsync()
        {          
            var PwdcomplexityRules =await Mediator.Send(new GetPwdRuleQuery());
            return Ok(PwdcomplexityRules);
        }

         [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var  PwdComplexity = await Mediator.Send(new GetPwdComplexityRuleByIdQuery() {Id=id});
            if(PwdComplexity ==null)
            {
                BadRequest("ID in the URL does not match the command PwdComplexity.");               
            }
            return Ok(PwdComplexity);

        }

         [HttpPost]
         [Route("Create")]
        public async Task<IActionResult>CreateAsync([FromBody] CreatePasswordComplexityRuleCommand command)
        {
        var validationResult = await _createPasswordComplexityRuleCommand.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
        var createpasswordComplexityrule = await Mediator.Send(command);
        return Ok("Created Successfully");         
        }

          [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdatePasswordComplexityRuleCommand command)
    {
         var validationResult = await _updatepasswordComplexityRuleCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
        if (id != command.Id)
        {
            return BadRequest("PasswordComplexityRule Id Mismatch");
        }

        var UpdateDepartment = await Mediator.Send(command);
        return Ok("Updated Successfully");

    }

     [HttpPut("delete/{id}")]
        
        public async Task<IActionResult> Delete(int id,DeletePasswordComplexityRuleCommand deleteCommand)
        {
            // Console.WriteLine(deleteCommand.Id);      
         
    //      try
    // {
        await Mediator.Send(deleteCommand);
        return Ok($"Password complexity rule with ID {id} deleted successfully {deleteCommand.Id}");
    // }
    // catch (Exception ex)
    // {
    //     return BadRequest($"Error deleting password complexity rule: {ex.Message}");
    // }


        }
    }











    
}