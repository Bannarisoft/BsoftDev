using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Companies.Queries.GetCompanies;
using Core.Application.Companies.Commands.CreateCompany;
using Core.Application.Companies.Queries.GetCompanyById;
using Core.Application.Companies.Commands.UpdateCompany;
using Core.Application.Companies.Commands.DeleteCompany;
using Core.Application.Companies.Queries.GetCompanyAutoComplete;
using FluentValidation;
using Core.Application.Companies.Commands.UploadFileCompany;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class CompanyController : ApiControllerBase
    {
        
        private readonly IValidator<CreateCompanyCommand>  _CreateCompanyCommandvalidator;
        private readonly IValidator<UpdateCompanyCommand> _UpdateCompanyCommandvalidator;
        private readonly IValidator<UploadFileCompanyCommand> _UploadFileCompanyCommandvalidator;

        public CompanyController(ISender mediator, IValidator<CreateCompanyCommand> createCompanyCommandValidator, IValidator<UpdateCompanyCommand> updateCompanyCommandValidator, IValidator<UploadFileCompanyCommand> uploadFileCompanyCommandvalidator) 
        : base(mediator)
        {
            _CreateCompanyCommandvalidator = createCompanyCommandValidator;
            _UpdateCompanyCommandvalidator = updateCompanyCommandValidator;
            _UploadFileCompanyCommandvalidator = uploadFileCompanyCommandvalidator;
        }

        [HttpGet("GetAllCompaniesAsync")]
        
        public async Task<IActionResult> GetAllCompaniesAsync()
        {
            var companies = await Mediator.Send(new GetCompanyQuery());
            var activecompanies = companies.Data.ToList(); 

            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = activecompanies
            });
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCompanyCommand command)
        {
            var validationResult = await _CreateCompanyCommandvalidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest, 
                    message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
           
            var createdCompany = await Mediator.Send(command);
            if (createdCompany.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status201Created,
                    message = createdCompany.Message,  
                    errors = "", 
                    data = createdCompany.Data  
                });
            }
            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = createdCompany.Message, 
                errors = "" 
            });
            
        }
         [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            
            if (id <= 0)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    Message = "Invalid Company ID" 
                });
            }

            var company = await Mediator.Send(new GetCompanyByIdQuery() { CompanyId = id });
            if (company == null)
            {
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    Message = "Company not found" 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = company.Data
            });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateCompanyCommand command)
        {
            var validationResult = await _UpdateCompanyCommandvalidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    Message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
           
            var companyExists = await Mediator.Send(new GetCompanyByIdQuery { CompanyId = command.Company.Id });

             if (companyExists == null)
             {
                 return NotFound(new 
                 { 
                    StatusCode=StatusCodes.Status404NotFound, 
                    message = $"Company ID {command.Company.Id} not found.", 
                    errors = "" 
                }); 
             }
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
        [HttpPut("delete")]
        public async Task<IActionResult> Delete(DeleteCompanyCommand deleteCompanyCommand)
        {
           
           var updatedCompany = await Mediator.Send(deleteCompanyCommand);

            if(updatedCompany.IsSuccess)
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
         [HttpGet("GetCompany")]
        public async Task<IActionResult> GetCompany([FromQuery] string searchPattern)
        {
           
            var companies = await Mediator.Send(new GetCompanyAutoCompleteQuery {SearchPattern = searchPattern});
            return Ok(new 
            { StatusCode=StatusCodes.Status200OK, 
            data = companies.Data 
            });
        }
        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo(UploadFileCompanyCommand uploadFileCompanyCommand)
        {
            var validationResult = await _UploadFileCompanyCommandvalidator.ValidateAsync(uploadFileCompanyCommand);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest, 
                    message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
            var file = await Mediator.Send(uploadFileCompanyCommand);
            if (!file.IsSuccess)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest, 
                    message = file.Message, 
                    errors = "" 
                });
            }
           
               
           return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = file.Message, 
                data = file.Data,
                errors = "" 
            });
              

        }
     
    }
}