using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetLocation.Commands.CreateAssetLocation;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.AssetLocation.Queries.GetAssetLocationById;
using Core.Application.AssetMaster.AssetLocation.Commands.UpdateAssetLocation;
using Core.Application.AssetMaster.AssetLocation.Queries.GetCustodian;
using Core.Application.AssetMaster.AssetLocation.Queries.GetSubLocationById;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategoriesById;
using FAM.API.Validation.AssetMaster.AssetLocation;
using FAM.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.AssetMaster
{
    [Route("[controller]")]
    public class AssetLocationController : ApiControllerBase
    {  
         private  readonly IValidator<CreateAssetLocationCommand>  _createAssetLocationCommandValidator;
         private readonly IValidator<UpdateAssetLocationCommand> _updateAssetLocationCommandValidator;
        private readonly ILogger<AssetLocationController> _logger;
      
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;

        public AssetLocationController( ILogger<AssetLocationController> logger,IMediator mediator, ApplicationDbContext dbContext,
        IValidator<CreateAssetLocationCommand> createAssetLocationCommandValidator, IValidator<UpdateAssetLocationCommand> updateAssetLocationCommandValidator) : base(mediator)
        
        {
            _logger = logger;
            _dbContext = dbContext;
            _mediator = mediator;
            _createAssetLocationCommandValidator = createAssetLocationCommandValidator;
            _updateAssetLocationCommandValidator = updateAssetLocationCommandValidator;

        }
        [HttpGet]
       
        public async Task<IActionResult> GetAllAssetLocationAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var assetgroups = await Mediator.Send(
            new GetAssetLocationQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
      
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = assetgroups.Data,
                TotalCount = assetgroups.TotalCount,
                PageNumber = assetgroups.PageNumber,
                PageSize = assetgroups.PageSize
                });
        }
         [HttpGet("{id}")]
         [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var assetLocation = await Mediator.Send(new GetAssetLocationByIdQuery() { Id = id});
           
             if(assetLocation.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetLocation.Data,message = assetLocation.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetLocation.Message });

        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateAssetLocationCommand command)
        {
            
            var validationResult = await _createAssetLocationCommandValidator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            var response = await Mediator.Send(command);

            if (response.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status201Created,
                    message = response.Message, 
                    data = response.Data
                });
            }

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = response.Message,
                Errors = ""
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateAssetLocationCommand command)
        {
            // Validate the command
            var validationResult = await _updateAssetLocationCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    StatusCode = StatusCodes.Status400BadRequest, 
                    Message = "Validation Failed", 
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            // Check if the AssetLocation exists
            var assetLocationExists = await Mediator.Send(new GetAssetLocationByIdQuery { Id = command.AssetId });
            if (assetLocationExists == null)
            {
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound, 
                    Message = $"AssetLocation ID {command.AssetId} not found.", 
                    Errors = "" 
                });
            }

            // Update the AssetLocation
            var response = await Mediator.Send(command);
            if (response.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode = StatusCodes.Status200OK, 
                    Message = response.Message, 
                    Errors = "" 
                });
            }

            // If update failed
            return BadRequest(new 
            { 
                StatusCode = StatusCodes.Status400BadRequest, 
                Message = response.Message, 
                Errors = "" 
            });
        }
        [HttpGet]  
        [Route("GetAllCustodian")]      
         public async Task<IActionResult> GetAllCustodianAsync([FromQuery] string OldUnitId,[FromQuery] string? SearchEmployee = null)
        {
           var custodian = await Mediator.Send(
            new GetCustodianQuery
            {
                OldUnitId = OldUnitId,                 
                SearchEmployee = SearchEmployee
              
            });
      
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = custodian.Data,
                SearchEmployee = SearchEmployee
            });
        }

        [HttpGet("AssetSubLocation/{id}/")]
        public async Task<IActionResult>GetSubLocationByIdAsync(int id)
        {
           
            var assetLocation = await Mediator.Send(new GetSubLocationByIdQuery() { Id = id });

           
             if(assetLocation.IsSuccess && assetLocation.Data != null)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetLocation.Data,message = assetLocation.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetLocation.Message });

        }
         

       
    }
}