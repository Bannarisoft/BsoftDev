using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.EntityLevelAdmin.Commands.CreateEntityLevelAdmin;
using Core.Application.EntityLevelAdmin.Commands.ResetPassword;
using Core.Application.EntityLevelAdmin.Commands.SendOTP;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Validation.Admin;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ApiControllerBase
    {
        private readonly IValidator<ResetPasswordCommand> _setAdminPasswordCommandValidator;
        public AdminController(ISender mediator, IValidator<ResetPasswordCommand> setAdminPasswordCommandValidator) 
        : base(mediator)
        {
            _setAdminPasswordCommandValidator = setAdminPasswordCommandValidator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateEntityLevelAdminCommand command)
        {
            
            var response = await Mediator.Send(command);
            if(response.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status201Created, 
                    message = response.Message
                });
            }
             

            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = response.Message, 
                errors = "" 
            }); 
            
        }
         [HttpPost("SendOTP")]
        public async Task<IActionResult> SendOTP(SendOTPCommand command)
        {
             
            var response = await Mediator.Send(command);
            if(response.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status200OK, 
                    message = response.Message,
                    data =response.Data
                });
            }
             

            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = response.Message, 
                errors = "" 
            }); 
            
        }
         [HttpPut("SetAdminPassword")]
        public async Task<IActionResult> SetAdminPassword(ResetPasswordCommand command)
        {
              var validationResult = await _setAdminPasswordCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest, 
                    message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
            
            var response = await Mediator.Send(command);
            if(response.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status200OK, 
                    message = response.Message
                });
            }
             

            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = response.Message, 
                errors = "" 
            }); 
            
        }
    }
}