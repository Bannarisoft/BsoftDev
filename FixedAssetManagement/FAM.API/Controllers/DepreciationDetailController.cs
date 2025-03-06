

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
        public async Task<IActionResult> DepreciationCalculateAsync([FromQuery] int companyId,int unitId,string finYear ,DateTimeOffset startDate,DateTimeOffset endDate,string depreciationType,int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        { 
             var assetMaster = await Mediator.Send(
                new GetDepreciationDetailQuery
                {
                    companyId=companyId,
                    unitId=unitId,
                    finYear=finYear,
                    startDate=startDate,
                    endDate=endDate,
                    depreciationType=depreciationType,
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
          [HttpDelete("{id}")]        
        public async Task<IActionResult> DeleteAsync(int id)
        {             
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Asset ID"
                });
            }            
              var result = await Mediator.Send(new DeleteDepreciationDetailCommand { Id = id });                 
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
                data =$"DepreciationGroup ID {id} Deleted" ,
                message = result.Message
            });
        }
                         
    }
}