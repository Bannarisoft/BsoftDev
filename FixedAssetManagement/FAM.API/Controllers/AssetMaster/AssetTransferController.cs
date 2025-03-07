using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecificationById;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.AssetMaster
{
    [Route("[controller]")]
    public class AssetTransferController : ApiControllerBase
    
    {
        private readonly ILogger<AssetTransferController> _logger;

        public AssetTransferController(ISender mediator )  : base(mediator)
        {
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var assetInsurances = await Mediator.Send(
                new AssetTransferQuery
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



         [HttpGet("{id}")]  
        [ActionName(nameof(GetByIdAsync))]        
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = "Invalid SpecificationMaster Id" 
                });
            }
            var result = await Mediator.Send(new GetAssetSpecificationByIdQuery { Id = id });            
            if (result is null )
            {                
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    message = $"SpecificationMaster {id} not found", 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = result.Data
            });   
        }

    }
}