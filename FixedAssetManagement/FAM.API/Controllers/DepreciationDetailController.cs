

using System.Globalization;
using Core.Application.DepreciationDetail.Commands.CreateDepreciationDetail;
using Core.Application.DepreciationDetail.Commands.DeleteDepreciationDetail;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepreciationDetailController  : ApiControllerBase
    {
         public DepreciationDetailController(ISender mediator) 
        : base(mediator) { }
        [HttpGet]
        public async Task<IActionResult> DepreciationCalculateAsync( [FromQuery] int companyId, 
        [FromQuery] int unitId, 
        [FromQuery] string finYear,
        [FromQuery] string? startDate,
        [FromQuery] string? endDate,
        [FromQuery] string depreciationType,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? searchTerm,
        [FromQuery] int depreciationPeriod)
        { 
            // Convert string dates to DateTimeOffset?
    DateTimeOffset? parsedStartDate = null;
    DateTimeOffset? parsedEndDate = null;

    if (!string.IsNullOrWhiteSpace(startDate))  // Allow null or empty values
    {
        if (!DateTimeOffset.TryParse(startDate, out var parsedDate))
        {
            return BadRequest(new { message = "Invalid startDate format. Use yyyy-MM-dd." });
        }
        parsedStartDate = parsedDate;
    }

    if (!string.IsNullOrWhiteSpace(endDate))  // Allow null or empty values
    {
        if (!DateTimeOffset.TryParse(endDate, out var parsedDate))
        {
            return BadRequest(new { message = "Invalid endDate format. Use yyyy-MM-dd." });
        }
        parsedEndDate = parsedDate;
    }
             var assetMaster = await Mediator.Send(
                new GetDepreciationDetailQuery
                {
                    companyId=companyId,
                    unitId=unitId,
                    finYear=finYear,
                    startDate=parsedStartDate,
                    endDate=parsedEndDate,
                    depreciationType=depreciationType,
                    PageNumber = pageNumber, 
                    PageSize = pageSize, 
                    SearchTerm = searchTerm,                    
                    depreciationPeriod=depreciationPeriod
                });
            return Ok(new 
            { 
                StatusCode = StatusCodes.Status200OK, 
                message = assetMaster.Message,
                data = assetMaster.Data.ToList(),                
                TotalCount = assetMaster.TotalCount,
                PageNumber = assetMaster.PageNumber,
                PageSize = assetMaster.PageSize
            });
        }
        [HttpPost]               
        public async Task<IActionResult> CreateAsync(CreateDepreciationDetailCommand  command)
        {             
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status201Created,
                    message = result.Message, 
                    data = result.Data
                });
            }  
            else
            {      
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = result.Message
                });
            } 
        }        
        [HttpDelete]        
        public async Task<IActionResult> DeleteAsync(DeleteDepreciationDetailCommand  command)
        {       
           /*  if (!DateTimeOffset.TryParseExact(startDate, new[] { "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy" }, 
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsedStartDate))
            {
                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = "Invalid startDate format. Use yyyy-MM-dd." });
            }

            if (!DateTimeOffset.TryParseExact(endDate, new[] { "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy" }, 
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsedEndDate))
            {
                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = "Invalid endDate format. Use yyyy-MM-dd." });
            }
                  */   
            var result = await Mediator.Send(new DeleteDepreciationDetailCommand { companyId=command.companyId,unitId=command.unitId,finYear=command.finYear,depreciationType=command.depreciationType,depreciationPeriod=command.depreciationPeriod});                 
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
                data =$"Depreciation Details Deleted" ,
                message = result.Message
            });
        }
                         
    }
}