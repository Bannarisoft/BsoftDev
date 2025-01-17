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
            var activecompanies = companies.Where(c => c.IsActive == 1).ToList(); 
            // var companies = await Mediator.Send(new GetCompanyQuery());
            // return Ok(companies);
            return Ok(activecompanies);
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateCompanyCommand command)
        {
            var validationResult = await _CreateCompanyCommandvalidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            if(command.Company.File ==null && command.Company.File.Length ==0)
            {
                return BadRequest("Invalid file");
            }
            
            var createdCompany = await Mediator.Send(command);
            if (createdCompany > 0)
            {
                return Ok(createdCompany);
            }
            return BadRequest("Error");
            
        }
         [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            // var basePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "AllFiles");
           
            // var company = await Mediator.Send(new GetCompanyByIdQuery() { CompanyId = id});
            // if(company == null)
            // {
            //     return NotFound();
            // }
            //  var filePath = Path.Combine(basePath, company.Logo);
            //  if (!System.IO.File.Exists(filePath))
            // {
            //     return NotFound(new { Message = "File not found" });
            // }
            // var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            // var contentType = GetContentType(filePath);
            //   return Ok(new
            //      {
            //          Company = company,
            //          FileBytes = fileBytes,
            //          ContentType = contentType,
            //          FileName = company.Logo
            //      });
          
            if (id <= 0)
            {
                return BadRequest("Invalid company ID");
            }

            var company = await Mediator.Send(new GetCompanyByIdQuery() { CompanyId = id });
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update([FromForm]  UpdateCompanyCommand command, int id)
        {
            var validationResult = await _UpdateCompanyCommandvalidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            if(command.Company.File ==null && command.Company.File.Length ==0)
            {
                return BadRequest("Invalid file");
            }
            

            if(id != command.Company.Id)
            {
                return BadRequest();
            }
            
           var updatedCompany = await Mediator.Send(command);

            if (updatedCompany)
            {
                return Ok("Updated Successfully");
            }
            
            return BadRequest("Error");
        }
        [HttpPut("delete/{id}")]
        public async Task<IActionResult> Delete(int id,DeleteCompanyCommand deleteCompanyCommand)
        {
            Console.WriteLine(id);
              if(id == 0)
            {
                return BadRequest();
            }
            await Mediator.Send(deleteCompanyCommand);

            return NoContent();
        }
         [HttpGet("GetCompany")]
        public async Task<IActionResult> GetCompany([FromQuery] string searchPattern)
        {
           
            var companies = await Mediator.Send(new GetCompanyAutoCompleteQuery {SearchPattern = searchPattern});
            return Ok(companies);
        }
         private string GetContentType(string path)
         {
             var ext = Path.GetExtension(path).ToLowerInvariant();
             return ext switch
             {
                 ".jpg" => "image/jpeg",
                 ".png" => "image/png",
                 ".gif" => "image/gif",
                 ".pdf" => "application/pdf",
                 ".txt" => "text/plain",
                 ".doc" => "application/msword",
                 ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                 _ => "application/octet-stream", // Default binary type
             };
         }
        //  private (bool IsSuccess, string FilePath, string ErrorMessage) HandleFileUpload(IFormFile file)
        //  {
        //       try
        //       {
        //           var folderName = Path.Combine("Resources", "AllFiles");
        //           var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        //           if (!Directory.Exists(pathToSave))
        //           {
        //               Directory.CreateDirectory(pathToSave);
        //           }

        //           var fileName = file.FileName;
        //           var fullPath = Path.Combine(pathToSave, fileName);

        //           if (System.IO.File.Exists(fullPath))
        //           {
        //               return (false, null, "File already exists");
        //           }

        //           using (var stream = new FileStream(fullPath, FileMode.Create))
        //           {
        //               file.CopyTo(stream);
        //           }

        //           return (true, fullPath, null); // Success result
        //       }
        //       catch (Exception ex)
        //       {
        //           return (false, null, $"An error occurred while uploading the file: {ex.Message}");
        //       }
        //  }
    }
}