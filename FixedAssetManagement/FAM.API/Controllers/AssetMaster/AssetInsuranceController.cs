using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetInsurance.Commands.CreateAssetInsurance;
using Core.Application.AssetMaster.AssetInsurance.Commands.DeleteAssetInsurance;
using Core.Application.AssetMaster.AssetInsurance.Commands.UpdateAssetInsurance;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsuranceById;
using FAM.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.AssetMaster
{
    [Route("api/[controller]")]
    public class AssetInsuranceController : ApiControllerBase
    {        
        private  readonly IValidator<CreateAssetInsuranceCommand>  _createAssetInsuranceCommandValidator;
         private readonly IValidator<UpdateAssetInsuranceCommand> _updateAssetInsuranceCommandValidator;      
       

        public AssetInsuranceController(ISender mediator,IValidator<CreateAssetInsuranceCommand> createAssetInsuranceCommandValidator, IValidator<UpdateAssetInsuranceCommand> updateAssetInsuranceCommandValidator) : base(mediator)
        {
            _createAssetInsuranceCommandValidator = createAssetInsuranceCommandValidator;
            _updateAssetInsuranceCommandValidator = updateAssetInsuranceCommandValidator;
           
        }

        [HttpGet("{id}")]         
        public async Task<IActionResult> GetByAssetIdAsync(int id)
        {
             var assetInsurance = await Mediator.Send(new GetAssetInsuranceByIdQuery() { Id = id});
           
             if(assetInsurance.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetInsurance.Data,message = assetInsurance.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetInsurance.Message });

           
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateAssetInsuranceCommand command)
        {
            var validationResult = await _createAssetInsuranceCommandValidator.ValidateAsync(command);

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
                return StatusCode(StatusCodes.Status201Created, new 
                { 
                    StatusCode = StatusCodes.Status201Created,
                    Message = response.Message,
                    Data = response.Data
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
            public async Task<IActionResult> Update(UpdateAssetInsuranceCommand command)
            {
                // Validate the command
                var validationResult = await _updateAssetInsuranceCommandValidator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new 
                    { 
                        StatusCode = StatusCodes.Status400BadRequest, 
                        Message = "Validation Failed", 
                        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                    });
                }

                // Check if the AssetInsurance exists
                var assetInsuranceExists = await Mediator.Send(new GetAssetInsuranceByIdQuery { Id = command.Id });
                if (assetInsuranceExists == null)
                {
                    return NotFound(new 
                    { 
                        StatusCode = StatusCodes.Status404NotFound, 
                        Message = $"AssetInsurance ID {command.Id} not found.", 
                        Errors = "" 
                    });
                }

                // Update the AssetInsurance
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

                return BadRequest(new 
                { 
                    StatusCode = StatusCodes.Status400BadRequest, 
                    Message = response.Message, 
                    Errors = "" 
                });
            }

         [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
              if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Asset ID"
                });
            }            
            var result = await Mediator.Send(new DeleteAssetInsuranceCommand { Id = id });                 
            if (!result.IsSuccess)
            {                
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data =$"Asset Insurance ID {id} Deleted" ,
                message = result.Message
            });
        }   

        [HttpGet]
        public async Task<IActionResult> GetAllAssetInsuranceAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var assetInsurances = await Mediator.Send(
                new GetAssetInsuranceQuery
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm
                });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = assetInsurances.Data,
                TotalCount = assetInsurances.TotalCount,
                PageNumber = assetInsurances.PageNumber,
                PageSize = assetInsurances.PageSize
            });
        }








    }
}