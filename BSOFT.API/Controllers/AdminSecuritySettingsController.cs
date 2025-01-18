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
using Core.Application.AdminSecuritySettings.Commands.UpdateAdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Commands.DeleteAdminSecuritySettings;
using AutoMapper;
using Core.Domain.Entities;


namespace BSOFT.API.Controllers
{
    [Route("[controller]")]
    public class AdminSecuritySettingsController : ApiControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<CreateAdminSecuritySettingsCommand> _createAdminSecuritySettingsCommandValidator;   
    private readonly IValidator <UpdateAdminSecuritySettingsCommand> _updateAdminSecuritysettingsCommandValidator;
      



        public AdminSecuritySettingsController (ISender mediator  , ApplicationDbContext dbContext,
        IValidator<CreateAdminSecuritySettingsCommand> createAdminSecuritySettingsCommandValidator, IValidator<UpdateAdminSecuritySettingsCommand> UpdateAdminSecuritySettingsCommandValidator )  : base(mediator)
        {
             _dbContext = dbContext; 
             _createAdminSecuritySettingsCommandValidator = createAdminSecuritySettingsCommandValidator;
             _updateAdminSecuritysettingsCommandValidator = UpdateAdminSecuritySettingsCommandValidator;

             
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
         
           [HttpPut("update/{id}")]
            public async Task<IActionResult> UpdateAsync(int id, UpdateAdminSecuritySettingsCommand command)
            {
                var validationResult = await _updateAdminSecuritysettingsCommandValidator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                return BadRequest(validationResult.Errors);
                }
                if (id != command.Id)
                {
                    return BadRequest("Admin Security Settings Id Mismatch");
                }
                var UpdateDepartment = await Mediator.Send(command);
                return Ok("Updated Successfully");
            }
            
             [HttpPut("delete/{id}")]        
        public async Task<IActionResult> Delete(int id, DeleteAdminSecuritySettingsCommand deleteAdminSecuritySettingCommand)
        {
             if(id != deleteAdminSecuritySettingCommand.Id)
            {
                return BadRequest();
            }
            await Mediator.Send(deleteAdminSecuritySettingCommand);

            return Ok("Status Closed Successfully");
        }


    }
}