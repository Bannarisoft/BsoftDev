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
using AutoMapper;
using Core.Domain.Entities;

namespace BSOFT.API.Controllers
{
    [Route("[controller]")]
    public class AdminSecuritySettingsController : ApiControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<CreateAdminSecuritySettingsCommand> _createAdminSecuritySettingsCommandValidator;   
      



        public AdminSecuritySettingsController (ISender mediator  , ApplicationDbContext dbContext,
        IValidator<CreateAdminSecuritySettingsCommand> createAdminSecuritySettingsCommandValidator )  : base(mediator)
        {
             _dbContext = dbContext; 
             _createAdminSecuritySettingsCommandValidator = createAdminSecuritySettingsCommandValidator;
             
        }

        [HttpGet]
       public async Task<IActionResult> GetAllAdminSecuritySettingsAsync( )
        {          
            var AdminSecuritySettings =await Mediator.Send(new GetAdminSecuritySettingsQuery());
            return Ok(AdminSecuritySettings);
        }

         [HttpPost]
         [Route("Create")]
        public async Task<IActionResult>CreateAsync([FromBody] CreateAdminSecuritySettingsCommand command)
        {
            Console.WriteLine("AdminSecuritySettings Create");

            var validationResult = await _createAdminSecuritySettingsCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
      var CreateAdminSecuritySettings = await Mediator.Send(command);
       // await Mediator.Send(command);

          return Ok("Created Successfully");      
        }
    }
}