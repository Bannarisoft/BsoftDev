using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetInsurance.Commands.CreateAssetInsurance;
using Core.Application.AssetMaster.AssetInsurance.Commands.UpdateAssetInsurance;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsuranceById;
using FAM.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.AssetMaster
{
    [Route("[controller]")]
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




    }
}