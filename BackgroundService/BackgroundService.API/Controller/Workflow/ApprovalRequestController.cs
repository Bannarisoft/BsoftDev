using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.API.Controller.Notification;
using BackgroundService.Application.Workflow.ApprovalRequests.Queries.GetAllApprovalRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundService.API.Controller.Workflow
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApprovalRequestController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public ApprovalRequestController(IMediator mediator)
        : base(mediator)
        {
            _mediator = mediator;
        }
         [HttpGet]
        public async Task<IActionResult> GetAllApprovalRequestAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var ApprovalRequest = await Mediator.Send(
            new GetAllApprovalRequestQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = ApprovalRequest.Data,
                TotalCount = ApprovalRequest.TotalCount,
                PageNumber = ApprovalRequest.PageNumber,
                PageSize = ApprovalRequest.PageSize
                });
        }
    }
}