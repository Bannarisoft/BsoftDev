using BSOFT.Infrastructure.Data;
using Core.Application.AuditLog.Commands.CreateAuditLog;
using Core.Application.AuditLog.Queries;
using Core.Application.AuditLog.Queries.GetAuditLog;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class AuditLogController : ApiControllerBase
    {
       private readonly ApplicationDbContext _dbContext;
         
       public AuditLogController(ISender mediator, 
                           ApplicationDbContext dbContext) 
         : base(mediator)
        {        
             _dbContext = dbContext;  
             
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAuditLogsAsync()
        {            

            var auditLogs = await Mediator.Send(new GetAuditLogQuery());
            //var activeCities = auditLogs.Where(c => c.IsActive == 1).ToList(); 
            return Ok(auditLogs);
        }

        
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateAuditLogCommand  command)
    {       
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
        return Ok(new { Message = "Log Entry created successfully", City = result.Data });
        }
        else
        {        
        return BadRequest(result.ErrorMessage);
        }        
    }
       [HttpGet("GetAuditLogSearch")]
        public async Task<IActionResult> GetAuditLog([FromQuery] string searchPattern)
        {
           
            var auditLogs = await Mediator.Send(new GetAuditLogBySearchPatternQuery {SearchPattern = searchPattern}); // Pass `searchPattern` to the constructor
            //var activeCities = cities.Where(c => c.IsActive == 1).ToList(); 
            return Ok(auditLogs);
        }

     
    }
}