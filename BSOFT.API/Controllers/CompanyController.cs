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

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllCompaniesAsync()
        {
           
            var companies = await Mediator.Send(new GetCompanyQuery());
            return Ok(companies);
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCompanyCommand command)
        {
            var createdCompany = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdCompany.Id }, createdCompany);
        }
         [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var company = await Mediator.Send(new GetCompanyByIdQuery() { CompanyId = id});
            if(company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCompanyCommand command )
        {
            if(id != command.Id)
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
    }
}