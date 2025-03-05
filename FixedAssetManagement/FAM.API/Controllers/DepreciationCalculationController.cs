

using Core.Application.DepreciationCalculation.Queries.GetDepreciationCalculation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepreciationCalculationController  : ApiControllerBase
    {
        public DepreciationCalculationController(ISender mediator) : base(mediator)
        {        
        }
        [HttpGet]
        public async Task<IActionResult> DepreciationCalculateAsync([FromQuery] int CompanyId,int UnitId,DateTimeOffset StartDate,DateTimeOffset EndDate, int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        { 
             var assetMaster = await Mediator.Send(
                new GetDepreciationCalculationQuery
                {
                    CompanyId=CompanyId,
                    UnitId=UnitId,
                    StartDate=StartDate,
                    EndDate=EndDate,
                    PageNumber = PageNumber, 
                    PageSize = PageSize, 
                    SearchTerm = SearchTerm
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
    }
}