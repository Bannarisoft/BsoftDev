using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.AssetMaster
{
    [Route("[controller]")]
    public class AssetInsuranceController : ControllerBase
    {
        private readonly ILogger<AssetInsuranceController> _logger;

        public AssetInsuranceController(ILogger<AssetInsuranceController> logger)
        {
            _logger = logger;
        }
[HttpGet]  // Or [HttpPost], depending on the function
    public IActionResult Index()
    {
        return Ok("AssetInsurance API is working!");
    }

    }
}