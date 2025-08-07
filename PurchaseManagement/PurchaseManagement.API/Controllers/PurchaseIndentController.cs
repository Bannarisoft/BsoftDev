using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PurchaseIndents.Command.CreatePurchaseIndent;
using Core.Application.PurchaseIndents.Command.DeletePurchaseIndent;
using Core.Application.PurchaseIndents.Command.UpdatePurchaseIndent;
using Core.Application.PurchaseIndents.Queries.GetAllPurchaseIndent;
using Core.Application.PurchaseIndents.Queries.GetPurchaseIndentById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PurchaseManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseIndentController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public PurchaseIndentController(IMediator mediator)
        : base(mediator)
        {

        }
        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseIndentAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var PurchaseIndent = await Mediator.Send(
             new GetAllPurchaseIndentQuery
             {
                 PageNumber = PageNumber,
                 PageSize = PageSize,
                 SearchTerm = SearchTerm
             });
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = PurchaseIndent.Data,
                TotalCount = PurchaseIndent.TotalCount,
                PageNumber = PurchaseIndent.PageNumber,
                PageSize = PurchaseIndent.PageSize
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreatePurchaseIndentCommand createPurchaseIndentCommand)
        {
            var CreatedIndent = await _mediator.Send(createPurchaseIndentCommand);
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message = "Created successfully.",
                data = CreatedIndent
            });

        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdatePurchaseIndentCommand updatePurchaseIndentCommand)
        {
            await _mediator.Send(updatePurchaseIndentCommand);
            return Ok(new
            {
                message = "Updated successfully.",
                statusCode = StatusCodes.Status200OK
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _mediator.Send(new DeletePurchaseIndentCommand { Id = id });
            return Ok(new
            {
                message = "Deleted successfully.",
                statusCode = StatusCodes.Status200OK
            });

        }
         [HttpGet("{id}")]        
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var Indent = await Mediator.Send(new GetPurchaseIndentByIdQuery() { Id = id});           
            return Ok(new { StatusCode=StatusCodes.Status200OK, data = Indent,message = "" });            
        }
    }
}