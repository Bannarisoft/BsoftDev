using Core.Application.CompanySettings.Commands.CreateCompanySettings;
using Core.Application.CompanySettings.Commands.UpdateCompanySettings;
using Core.Application.CompanySettings.Queries.GetCompanySettingsById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanySettingsController : ApiControllerBase
    {
        private readonly IValidator<CreateCompanySettingsCommand> _CreateCompanySettingsCommand;
        private readonly IValidator<UpdateCompanySettingsCommand> _UpdateCompanySettingsCommand;
        public CompanySettingsController(ISender mediator, IValidator<CreateCompanySettingsCommand> createCompanySettingsCommand, IValidator<UpdateCompanySettingsCommand> updateCompanySettingsCommand)
        : base(mediator)
        {
            _CreateCompanySettingsCommand = createCompanySettingsCommand;
            _UpdateCompanySettingsCommand = updateCompanySettingsCommand;
        }
          [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCompanySettingsCommand command)
        {
            var validationResult = await _CreateCompanySettingsCommand.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest, 
                    message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
            var createdCompanySetting = await Mediator.Send(command);
            if (createdCompanySetting.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status201Created,
                    message = createdCompanySetting.Message,  
                    errors = "", 
                    data = createdCompanySetting.Data  
                });
            }
            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = createdCompanySetting.Message, 
                errors = "" 
            });
            
        }
        
        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateCompanySettingsCommand command)
        {
            var validationResult = await _UpdateCompanySettingsCommand.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    Message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
           
            // var companySettingsExists = await Mediator.Send(new GetCompanySettingByIdQuery { Id = command.Id });

            //  if (companySettingsExists == null)
            //  {
            //      return NotFound(new 
            //      { 
            //         StatusCode=StatusCodes.Status404NotFound, 
            //         message = $"Company Setting ID {command.Id} not found.", 
            //         errors = "" 
            //     }); 
            //  }
           var updatedCompany = await Mediator.Send(command);

            if (updatedCompany.IsSuccess)
            {
                return Ok(new 
                {
                    StatusCode=StatusCodes.Status200OK,
                    message = updatedCompany.Message,
                    errors = ""
                });
            }
            
            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = updatedCompany.Message, 
                errors = "" 
            });
        }
           [HttpGet]
        public async Task<IActionResult> GetByIdAsync()
        {
            
            // if (id <= 0)
            // {
            //     return BadRequest(new 
            //     { 
            //         StatusCode=StatusCodes.Status400BadRequest,
            //         Message = "Invalid Company Setting ID" 
            //     });
            // }

            var company = await Mediator.Send(new GetCompanySettingByIdQuery() {  });
            if (company == null)
            {
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    Message = "Company Setting not found" 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = company.Data
            });
        }
    }
}