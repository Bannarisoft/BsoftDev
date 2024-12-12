using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BSOFT.Application.Companies.Queries.GetCompanies;
using BSOFT.Application.Companies.Commands.CreateCompany;
using BSOFT.Application.Companies.Queries.GetCompanyById;
using BSOFT.Application.Companies.Commands.UpdateCompany;
using BSOFT.Application.Companies.Commands.DeleteCompany;
using Microsoft.AspNetCore.Http;
using System.IO;
using BSOFT.Application.Companies.Queries.GetCompanyAutoComplete;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ApiControllerBase
    {
        [HttpGet("GetAllCompaniesAsync")]
        public async Task<IActionResult> GetAllCompaniesAsync()
        {
           
            var companies = await Mediator.Send(new GetCompanyQuery());
            return Ok(companies);
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateCompanyCommand command)
        {
            if(command.File ==null && command.File.Length ==0)
            {
                return BadRequest("Invalid file");
            }
            var uploadResult = HandleFileUpload(command.File);
            if (!uploadResult.IsSuccess)
            {
                return BadRequest(uploadResult.ErrorMessage);
            }
            command.Logo =uploadResult.FilePath;
            var createdCompany = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetByIdAsync), new { CoId = createdCompany.CoId }, createdCompany);
        }
         [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "AllFiles");
           
            var company = await Mediator.Send(new GetCompanyByIdQuery() { CompanyId = id});
            if(company == null)
            {
                return NotFound();
            }
             var filePath = Path.Combine(basePath, company.Logo);
             if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { Message = "File not found" });
            }
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(filePath);
              return Ok(new
                 {
                     Company = company,
                     FileBytes = fileBytes,
                     ContentType = contentType,
                     FileName = company.Logo
                 });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCompanyCommand command )
        {
            if(id != command.CoId)
            {
                return BadRequest();
            }
            await Mediator.Send(command);

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteCompanyCommand {Id=id });

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
         private (bool IsSuccess, string FilePath, string ErrorMessage) HandleFileUpload(IFormFile file)
         {
              try
              {
                  var folderName = Path.Combine("Resources", "AllFiles");
                  var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                  if (!Directory.Exists(pathToSave))
                  {
                      Directory.CreateDirectory(pathToSave);
                  }

                  var fileName = file.FileName;
                  var fullPath = Path.Combine(pathToSave, fileName);

                  if (System.IO.File.Exists(fullPath))
                  {
                      return (false, null, "File already exists");
                  }

                  using (var stream = new FileStream(fullPath, FileMode.Create))
                  {
                      file.CopyTo(stream);
                  }

                  return (true, fullPath, null); // Success result
              }
              catch (Exception ex)
              {
                  return (false, null, $"An error occurred while uploading the file: {ex.Message}");
              }
         }
    }
}