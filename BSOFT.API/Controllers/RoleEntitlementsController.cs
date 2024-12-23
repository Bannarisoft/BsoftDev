using BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using BSOFT.Application.Common.Interfaces;
// using BSOFT.Application.RoleEntitlements.Queries.GetRoles;
using BSOFT.Application.Role.Queries.GetRolesAutocomplete;
using BSOFT.Application.Role.Queries.GetRoleById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    
    public class RoleEntitlementsController : ApiControllerBase
    {
        private readonly IRoleEntitlementRepository _repository;
        public RoleEntitlementsController(ISender mediator, IRoleEntitlementRepository repository) : base(mediator)
        {
            _repository = repository;
        }

    [HttpPost]
    public async Task<IActionResult> CreateRoleEntitlement(CreateRoleEntitlementCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(new { Message = "Role Entitlement created successfully.", CreatedCount = result });
    }

    [HttpGet("modules")]
    public async Task<IActionResult> GetModulesWithMenus()
    {
        var modules = await _repository.GetModulesWithMenusAsync();
        return Ok(modules.Select(module => new
        {
            module.Id,
            module.Name,
            Menus = module.Menus.Select(menu => new { menu.Id, menu.Name })
        }));
    }

    }
}
