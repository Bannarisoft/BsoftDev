using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Language.Commands.CreateLanguage;
using Core.Application.Language.Commands.DeleteLanguage;
using Core.Application.Language.Commands.UpdateLanguage;
using Core.Application.Language.Queries.GetLanguageAutoComplete;
using Core.Application.Language.Queries.GetLanguageById;
using Core.Application.Language.Queries.GetLanguages;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController : ApiControllerBase
    {
        private readonly IValidator<CreateLanguageCommand> _createLanguageCommandValidator;
        private readonly IValidator<UpdateLanguageCommand> _updateLanguageCommandValidator;
        public LanguageController(ISender mediator, IValidator<CreateLanguageCommand> createLanguageCommandValidator, IValidator<UpdateLanguageCommand> updateLanguageCommandValidator) 
        : base(mediator)
        {
            _createLanguageCommandValidator = createLanguageCommandValidator;
            _updateLanguageCommandValidator = updateLanguageCommandValidator;
        }
          [HttpGet]
        public async Task<IActionResult> GetAllLanguagesAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var languages = await Mediator.Send(
            new GetLanguageQuery
           {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK,
                 data = languages.Data.ToList(),
                TotalCount = languages.TotalCount,
                 PageNumber = languages.PageNumber,
                 PageSize = languages.PageSize
                 });
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateLanguageCommand command)
        {
            
            var validationResult = await _createLanguageCommandValidator.ValidateAsync(command);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
            var response = await Mediator.Send(command);
            if(response.IsSuccess)
            {
                return Ok(new { StatusCode=StatusCodes.Status201Created, message = response.Message, errors = "", data = response.Data });
            }
             

            return BadRequest( new { StatusCode=StatusCodes.Status400BadRequest, message = response.Message, errors = "" }); 
            
        }
         [HttpGet("{id}")]
         [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
           
            var language = await Mediator.Send(new GetLanguageByIdQuery() { Id = id});
          
             if(language == null)
            {
                return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = $"Language ID {id} not found.", errors = "" });
            }
            return Ok(new { StatusCode=StatusCodes.Status200OK, data = language.Data});
        }

        [HttpPut]
        public async Task<IActionResult> Update( UpdateLanguageCommand command )
        {
            
            var validationResult = await _updateLanguageCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
          

             var languageExists = await Mediator.Send(new GetLanguageByIdQuery { Id = command.Id });

             if (languageExists == null)
             {
                 return NotFound(new { StatusCode=StatusCodes.Status404NotFound, message = $"Language ID {command.Id} not found.", errors = "" }); 
             }

             var response = await Mediator.Send(command);
             if(response.IsSuccess)
             {
                 return Ok(new { StatusCode=StatusCodes.Status200OK, message = response.Message, errors = "" });
             }
            
           

            return BadRequest( new { StatusCode=StatusCodes.Status400BadRequest, message = response.Message, errors = "" }); 
        }


        [HttpPut("{id}")]
        
        public async Task<IActionResult> Delete(int id)
        {
           
           var updatedLanguage = await Mediator.Send(new DeleteLanguageCommand { Id = id });

           if(updatedLanguage.IsSuccess)
           {
            return Ok(new { StatusCode=StatusCodes.Status200OK, message = updatedLanguage.Message, errors = "" });
              
           }

            return BadRequest(new { StatusCode=StatusCodes.Status400BadRequest, message = updatedLanguage.Message, errors = "" });
            
        }
         [HttpGet("by-name")]
        public async Task<IActionResult> GetLanguage([FromQuery] string? name)
        {
           
            var languages = await Mediator.Send(new GetLanguageAutoCompleteQuery {SearchPattern = name});
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = languages.Data });
        }
      
    }
}