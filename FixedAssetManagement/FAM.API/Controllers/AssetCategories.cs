using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetCategories.Command.CreateAssetCategories;
using Core.Application.AssetCategories.Command.DeleteAssetCategories;
using Core.Application.AssetCategories.Command.UpdateAssetCategories;
using Core.Application.AssetCategories.Queries.GetAssetCategories;
using Core.Application.AssetCategories.Queries.GetAssetCategoriesAutoComplete;
using Core.Application.AssetCategories.Queries.GetAssetCategoriesById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers
{
    [Route("[controller]")]
    public class AssetCategories :  ApiControllerBase
    {
        private readonly IValidator<CreateAssetCategoriesCommand> _createassetcategoriescommandvalidator;
        private readonly IValidator<UpdateAssetCategoriesCommand> _updateassetcategoriescommandvalidator;
        private readonly ILogger<AssetCategories> _logger;
         private readonly IMediator _mediator;

        public AssetCategories(ILogger<AssetCategories> logger, IMediator mediator, IValidator<CreateAssetCategoriesCommand> createassetcategoriescommandvalidator,IValidator<UpdateAssetCategoriesCommand> updateassetcategoriescommandvalidator )
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _createassetcategoriescommandvalidator=createassetcategoriescommandvalidator;
            _updateassetcategoriescommandvalidator=updateassetcategoriescommandvalidator;        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssetCategoriesAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var assetcategories = await Mediator.Send(
            new GetAssetCategoriesQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = assetcategories.Data,
                TotalCount = assetcategories.TotalCount,
                PageNumber = assetcategories.PageNumber,
                PageSize = assetcategories.PageSize
                });
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetAssetCategories([FromQuery] string? CategoryName)
        {
        var assetcategories = await Mediator.Send(new GetAssetCategoriesAutoCompleteQuery 
        { 
                SearchPattern = CategoryName ?? string.Empty 
        });

        return Ok(new { StatusCode = StatusCodes.Status200OK, data = assetcategories.Data });
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var assetcategories = await Mediator.Send(new GetAssetCategoriesByIdQuery() { Id = id});
          
            if(assetcategories.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetcategories.Data,message = assetcategories.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetcategories.Message });   
        }

[HttpPost]
public async Task<IActionResult> CreateAsync(CreateAssetCategoriesCommand createAssetCategoriesCommand)
{
     // Validate the incoming command
    var validationResult = await _createassetcategoriescommandvalidator.ValidateAsync(createAssetCategoriesCommand);
  
    if (!validationResult.IsValid)
    {
        
        return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = "Validation failed",
            errors = validationResult.Errors.Select(e => e.ErrorMessage)
        });
    }

    // Process the command
    var CreatedAssetCategoriesId = await _mediator.Send(createAssetCategoriesCommand);

    if (CreatedAssetCategoriesId.IsSuccess)
    {
     
      return Ok(new
      {
          StatusCode = StatusCodes.Status201Created,
          message =CreatedAssetCategoriesId.Message,
          data = CreatedAssetCategoriesId.Data
      });
    }
     
      return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = CreatedAssetCategoriesId.Message
        });
  
}
[HttpPut]
public async Task<IActionResult> UpdateAsync(UpdateAssetCategoriesCommand updateAssetCategoriesCommand)
{
  
        // Validate the incoming command
        var validationResult = await _updateassetcategoriescommandvalidator.ValidateAsync(updateAssetCategoriesCommand);
       
        if (!validationResult.IsValid)
        {
           
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var updatedassetcategories = await _mediator.Send(updateAssetCategoriesCommand);

        if (updatedassetcategories.IsSuccess)
        {
           
           return Ok(new
            {
                message = updatedassetcategories.Message,
                statusCode = StatusCodes.Status200OK
            });
        }
        
        return NotFound(new
        {
            message =updatedassetcategories.Message,
            statusCode = StatusCodes.Status404NotFound
        });   
}

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAssetCategoriesAsync(int id)
{

        // Process the delete command
        var result = await _mediator.Send(new DeleteAssetCategoriesCommand { Id = id });

        if (result.IsSuccess) 
        {
           
             return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK
            });
            
        }
      
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}


    }
}