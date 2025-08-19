using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PartyMaster.Command.CreatePartyMaster;
using Core.Application.PartyMaster.Command.DeletePartyMasterDocument;
using Core.Application.PartyMaster.Command.UpdatePartyMaster;
using Core.Application.PartyMaster.Command.UploadPartyMasterDocument;
using Core.Application.PartyMaster.Queries.GetPartMaster;
using Core.Application.PartyMaster.Queries.GetPartMasterAutoComplete;
using Core.Application.PartyMaster.Queries.GetPartyGroupLoad;
using Core.Application.PartyMaster.Queries.GetPartyMasterById;
using MassTransit.Futures.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PartyManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class PartyMasterController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public PartyMasterController(IMediator mediator)
        : base(mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("load")]
        public async Task<IActionResult> GetPartyGroups([FromQuery] string groupTypeIds)
        {
            if (string.IsNullOrWhiteSpace(groupTypeIds))
            {
                return BadRequest("GroupTypeIds are required.");
            }
            var parsedIds = groupTypeIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.Parse(id.Trim()))
                .ToList();

            var query = new GetPartyGroupLoadQuery { GroupTypeIds = parsedIds };

            var result = await _mediator.Send(query);

            if (result == null || !result.Any())
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    data = (object?)null,
                    message = $"PartyGroup with ID {groupTypeIds} not found"
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result,
                message = "ID fetched successfully"
            });

        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreatePartyMasterCommand createPartyMasterCommand)
        {

            // Process the command
            var CreatePartmasterId = await _mediator.Send(createPartyMasterCommand);

            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message = "Created successfully.",
                data = CreatePartmasterId
            });

        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdatePartyMasterCommand updatePartyMasterCommand)
        {

            await _mediator.Send(updatePartyMasterCommand);

            return Ok(new
            {
                message = "Updated successfully.",
                statusCode = StatusCodes.Status200OK
            });

        }

        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadDocument(UploadPartyMasterDocumentCommand uploadFileCommand)
        {

            var file = await Mediator.Send(uploadFileCommand);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = file,
                data = file,
                errors = ""
            });
        }
        // DELETE: api/PartyMasterDocument/delete-logo
        [HttpDelete("delete-document")]
        public async Task<IActionResult> DeleteDocument([FromBody] DeletePartyMasterDocumentCommand deleteFileCommand)
        {
            if (deleteFileCommand == null || string.IsNullOrWhiteSpace(deleteFileCommand.partydocumentPath))
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid request. 'PartyDocumentPath' cannot be null or empty.",
                    errors = ""
                });
            }
            var file = await Mediator.Send(deleteFileCommand);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "File deleted successfully.",
                errors = ""
            });
        }

        [HttpGet("{partyId}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int partyId)
        {
            var partyMaster = await Mediator.Send(new GetPartyMasterByIdQuery() { PartyId = partyId });

            if (partyMaster == null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    data = (object?)null,
                    message = $"PartyMaster with ID {partyId} not found"
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = partyMaster,
                message = "ID fetched successfully"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPartyMasterAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var partymaster = await Mediator.Send(
             new GetPartMasterQuery
             {
                 PageNumber = PageNumber,
                 PageSize = PageSize,
                 SearchTerm = SearchTerm
             });
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = partymaster.Data,
                TotalCount = partymaster.TotalCount,
                PageNumber = partymaster.PageNumber,
                PageSize = partymaster.PageSize
            });
        }

         [HttpGet("by-name")]
        public async Task<IActionResult> GetPartyMasterAutoComplete([FromQuery] string? Typename)
        {
            var PartyMaster = await Mediator.Send(new GetPartyMasterAutoCompleteQuery
            {
                SearchPattern = Typename ?? string.Empty
            });

            return Ok(new { StatusCode = StatusCodes.Status200OK, data = PartyMaster });
        }       
    }
}