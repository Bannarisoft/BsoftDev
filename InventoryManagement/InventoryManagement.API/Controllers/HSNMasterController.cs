using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IHSNMaster;
using Core.Application.HSNMaster.Command.CreateHSNMaster;
using Core.Application.HSNMaster.Command.DeleteHSNMaster;
using Core.Application.HSNMaster.Command.UpdateHSNMaster;
using Core.Application.HSNMaster.Queries.GetAllHSNMaster;
using Core.Application.HSNMaster.Queries.GetHSNMasterAutoComplete;
using Core.Application.HSNMaster.Queries.GetHSNMasterById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class HSNMasterController : ApiControllerBase
    {
        private readonly IHSNMasterQueryRepository _hSNMasterQueryRepository;

        public HSNMasterController(IMediator mediator, IHSNMasterQueryRepository hSNMasterQueryRepository) : base(mediator)
        {
            _hSNMasterQueryRepository = hSNMasterQueryRepository;

        }

        [HttpGet]

        public async Task<IActionResult> GetAllHSNMasterAsync(
            [FromQuery] int PageNumber,
            [FromQuery] int PageSize,
            [FromQuery] string? SearchTerm = null)
        {
            var hsnMaster = await Mediator.Send(new GetHSNMasterQuery
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchTerm = SearchTerm
            });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = hsnMaster.Data,
                TotalCount = hsnMaster.TotalCount,
                PageNumber = hsnMaster.PageNumber,
                PageSize = hsnMaster.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHSNMasterByIdAsync(int id)
        {
            var hsnMaster = await Mediator.Send(new GetHSNMasterByIdQuery { Id = id });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = hsnMaster.Data,
                message = hsnMaster.Message,
                isSuccess = hsnMaster.IsSuccess
            });
        }
        [HttpGet("by-name")]
        public async Task<IActionResult> GetHSNMasterAutoCompleteAsync([FromQuery] string? hsnCode)
        {
            var hsnList = await Mediator.Send(new GetHSNMasterAutoCompleteQuery
            {
                SearchPattern = hsnCode ?? string.Empty
            });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = hsnList
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateHSNMaster([FromBody] CreateHSNMasterCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await Mediator.Send(command);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                isSuccess = result.IsSuccess,
                message = result.Message,
                data = result.Data
            });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateHSNMaster(int id, [FromBody] UpdateHSNMasterCommand command)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid Id provided."
                });
            }

            // Ensure the command Id matches the route Id
            command.Id = id;

            // Send to MediatR handler
            var result = await Mediator.Send(command);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                isSuccess = result.IsSuccess,
                message = result.Message,
                data = result.Data
            });
        }
        
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteHSNMaster( int id)
         {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid Id provided."
                });
            }

            var command = new DeleteHSNMasterCommand  {
                Id = id,
              
            };

            var result = await Mediator.Send(command);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                isSuccess = result.IsSuccess,
                message = result.Message,
                data = result.Data
            });
        }

       
    }
}