using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SagaOrchestrator.Infrastructure.Services;

namespace SagaOrchestrator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly OrchestratorService _orchestratorService;

        public AssetController(OrchestratorService orchestratorService)
        {
            _orchestratorService = orchestratorService;
        }

        [HttpPost("asset")]
        public async Task<IActionResult> TriggerAsset(int userId, int assetId)
        {
            await _orchestratorService.TriggerAssetCreation(userId, assetId);
            return Ok(new
            {
                message = "Asset creation process triggered successfully.",
                userId = userId,
                assetId = assetId
            });
            // await _orchestratorService.TriggerAssetCreation(userId, assetId);
            // return Ok("Asset creation triggered.");
        }
    }
}