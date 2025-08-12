using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.API.Controller.Notification;
using BackgroundService.Application.Workflow.ApprovalRequests.Commands.ApproveApprovalRequest;
using BackgroundService.Application.Workflow.ApprovalRequests.Commands.ApproveDocumentUpload;
using BackgroundService.Application.Workflow.ApprovalRequests.Commands.RejectApprovalRequest;
using BackgroundService.Application.Workflow.ApprovalRequests.Queries.ApprovalDocumentDownload;
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
        public async Task<IActionResult> GetAllApprovalRequestAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var ApprovalRequest = await Mediator.Send(
             new GetAllApprovalRequestQuery
             {
                 PageNumber = PageNumber,
                 PageSize = PageSize,
                 SearchTerm = SearchTerm
             });
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = ApprovalRequest.Data,
                TotalCount = ApprovalRequest.TotalCount,
                PageNumber = ApprovalRequest.PageNumber,
                PageSize = ApprovalRequest.PageSize
            });
        }
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveAsync(ApproveApprovalRequestCommand approveApprovalRequestCommand)
        {
            var ApproveReq = await _mediator.Send(approveApprovalRequestCommand);
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message = "Approved successfully.",
                data = ApproveReq
            });

        }
        [HttpPost("reject")]
        public async Task<IActionResult> RejectAsync(RejectApprovalRequestCommand approveApprovalRequestCommand)
        {
            var ApproveReq = await _mediator.Send(approveApprovalRequestCommand);
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message = "Rejected successfully.",
                data = ApproveReq
            });

        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(UploadFileCommand uploadFileCommand)
        {
            var Result = await _mediator.Send(uploadFileCommand);
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message = "Upload successfully.",
                data = Result
            });

        }
        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string relativePath)
        {
            var result = await Mediator.Send(new DownloadFileQuery { RelativePath = relativePath });

            return File(result.FileBytes, result.ContentType, result.FileName);
        }
    }
}