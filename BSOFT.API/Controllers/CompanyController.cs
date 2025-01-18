using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Companies.Queries.GetCompanies;
using Core.Application.Companies.Commands.CreateCompany;
using Core.Application.Companies.Queries.GetCompanyById;
using Core.Application.Companies.Commands.UpdateCompany;
using Core.Application.Companies.Commands.DeleteCompany;
using Microsoft.AspNetCore.Http;
using System.IO;
using Core.Application.Companies.Queries.GetCompanyAutoComplete;
using Core.Application.Common.Interfaces;
using System.Text.Json;
using FluentValidation;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ApiControllerBase
    {
        
        private readonly IValidator<CreateCompanyCommand>  _CreateCompanyCommandvalidator;
        private readonly IValidator<UpdateCompanyCommand> _UpdateCompanyCommandvalidator;

        public CompanyController(ISender mediator, IValidator<CreateCompanyCommand> createCompanyCommandValidator, IValidator<UpdateCompanyCommand> updateCompanyCommandValidator) 
        : base(mediator)
        {
            _CreateCompanyCommandvalidator = createCompanyCommandValidator;
            _UpdateCompanyCommandvalidator = updateCompanyCommandValidator;
        }

        [HttpGet("GetAllCompaniesAsync")]
        public async Task<IActionResult> GetAllCompaniesAsync()
        {
            var companies = await Mediator.Send(new GetCompanyQuery());
            var activecompanies = companies.Data.Where(c => c.IsActive == 1).ToList(); 

            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = activecompanies
            });
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateCompanyCommand command)
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
        public async Task<IActionResult> Update([FromForm]  UpdateCompanyCommand command)
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
     
    }
}