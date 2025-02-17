using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetSubCategories.Command.CreateAssetSubCategories;
using Core.Application.AssetSubCategories.Command.DeleteAssetSubCategories;
using Core.Application.AssetSubCategories.Command.UpdateAssetSubCategories;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategories;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategoriesAutoComplete;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategoriesById;
using FAM.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers
{
    [Route("api/[controller]")]
    public class AssetSubCategoriesController  : ApiControllerBase
    {
        private readonly ILogger<AssetSubCategoriesController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateAssetSubCategoriesCommand> _createassetsubcategoriescommandvalidator;
        private readonly IValidator<UpdateAssetSubCategoriesCommand> _updateassetsubcategoriescommandvalidator;

        public AssetSubCategoriesController(ILogger<AssetSubCategoriesController> logger, IMediator mediator, ApplicationDbContext dbContext,IValidator<CreateAssetSubCategoriesCommand>  createassetsubcategoriescommandvalidator,IValidator<UpdateAssetSubCategoriesCommand> updateassetsubcategoriescommandvalidator)
        :base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _dbContext = dbContext;
            _createassetsubcategoriescommandvalidator=createassetsubcategoriescommandvalidator;
            _updateassetsubcategoriescommandvalidator=updateassetsubcategoriescommandvalidator;

        }
         [HttpGet]
        public async Task<IActionResult> GetAllAssetSubCategoriesAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var assetsubcategories = await Mediator.Send(
            new GetAssetSubCategoriesQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
      
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = assetsubcategories.Data,
                TotalCount = assetsubcategories.TotalCount,
                PageNumber = assetsubcategories.PageNumber,
                PageSize = assetsubcategories.PageSize
                });
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetAssetSubCategories([FromQuery] string? subcategoryname)
        {
        var assetsubcategoriesgroups = await Mediator.Send(new GetAssetSubCategoriesAutoCompleteQuery 
        { 
                SearchPattern = subcategoryname ?? string.Empty 
        });

        return Ok(new { StatusCode = StatusCodes.Status200OK, data = assetsubcategoriesgroups.Data });
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var assetsubcategoriesgroups = await Mediator.Send(new GetAssetSubCategoriesByIdQuery() { Id = id});
          
            if(assetsubcategoriesgroups.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetsubcategoriesgroups.Data,message = assetsubcategoriesgroups.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetsubcategoriesgroups.Message });
           
        }
        [HttpPost]
public async Task<IActionResult> CreateAsync(CreateAssetSubCategoriesCommand createAssetsubCategoriesCommand)
{
     // Validate the incoming command
    var validationResult = await _createassetsubcategoriescommandvalidator.ValidateAsync(createAssetsubCategoriesCommand);
  
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
    var CreatedAssetsubCategoriesId = await _mediator.Send(createAssetsubCategoriesCommand);

    if (CreatedAssetsubCategoriesId.IsSuccess)
    {
     
      return Ok(new
      {
          StatusCode = StatusCodes.Status201Created,
          message =CreatedAssetsubCategoriesId.Message,
          data = CreatedAssetsubCategoriesId.Data
      });
    }
     
      return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = CreatedAssetsubCategoriesId.Message
        });
  
}
[HttpPut]
public async Task<IActionResult> UpdateAsync(UpdateAssetSubCategoriesCommand updateAssetsubCategoriesCommand)
{
  
        // Validate the incoming command
        var validationResult = await _updateassetsubcategoriescommandvalidator.ValidateAsync(updateAssetsubCategoriesCommand);
       
        if (!validationResult.IsValid)
        {
           
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var updatedassetsubcategories = await _mediator.Send(updateAssetsubCategoriesCommand);

        if (updatedassetsubcategories.IsSuccess)
        {
           
           return Ok(new
            {
                message = updatedassetsubcategories.Message,
                statusCode = StatusCodes.Status200OK
            });
        }
        
        return NotFound(new
        {
            message =updatedassetsubcategories.Message,
            statusCode = StatusCodes.Status404NotFound
        });   
}

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAssetSubCategoriesAsync(int id)
{

        // Process the delete command
        var result = await _mediator.Send(new DeleteAssetSubCategoriesCommand { Id = id });

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
