using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IWarehouseMaster;
using Core.Application.WarehouseMaster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WarehouseManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class WarehouseMasterController : ApiControllerBase
    {
        private readonly IWarehouseMasterQueryRepository _warehouseMasterQueryRepository;

        public WarehouseMasterController(ISender mediator, IWarehouseMasterQueryRepository warehouseMaster) : base(mediator)
        {
            _warehouseMasterQueryRepository = warehouseMaster;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWarehouseMasterAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {

            var warehousemaster = await Mediator.Send(
                new GetAllWarehouseMastersQuery
                {
                    PageNumber = PageNumber, 
                    PageSize = PageSize, 
                    SearchTerm = SearchTerm
                });
            // var activecompanies = companies.Data.ToList(); 

            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = warehousemaster.Data,
                TotalCount = warehousemaster.TotalCount,
                PageNumber = warehousemaster.PageNumber,
                PageSize = warehousemaster.PageSize
            });
        }
        
       

      
    }
}