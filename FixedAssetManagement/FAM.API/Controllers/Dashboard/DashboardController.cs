using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.Dashboard
{
    [Route("[controller]")]
    public class DashboardController : Controller
    {
        private readonly IDashboardQueryRepository _dashboardQueryRepository ;
          private readonly IHttpContextAccessor _httpContextAccessor;


        public DashboardController(IDashboardQueryRepository dashboardQueryRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dashboardQueryRepository = dashboardQueryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

       [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var result = await _dashboardQueryRepository.GetDashboardDataAsync(); // No unitId passed explicitly
                return Ok(new
                {
                    isSuccess = true,
                    message = "Dashboard data retrieved successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Failed to retrieve dashboard data.",
                    error = ex.Message
                });
            }
        }
    }
}