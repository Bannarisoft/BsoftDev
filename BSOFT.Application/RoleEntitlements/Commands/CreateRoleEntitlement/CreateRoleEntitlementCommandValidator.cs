using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement
{
    public class CreateRoleEntitlementCommandValidator : AbstractValidator<CreateRoleEntitlementCommand>
    {
    public CreateRoleEntitlementCommandValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty().WithMessage("Role Name is required.");
        RuleFor(x => x.MenuPermissions).NotEmpty().WithMessage("Menu Permissions are required.");
        RuleForEach(x => x.MenuPermissions).SetValidator(new MenuPermissionValidator());
    }
    public class MenuPermissionValidator : AbstractValidator<MenuPermissionDto>
    {
    public MenuPermissionValidator()
    {
        RuleFor(x => x.MenuName).NotEmpty().WithMessage("Menu Name is required.");
        RuleFor(x => x).Must(p => p.CanView || p.CanAdd || p.CanUpdate || p.CanDelete || p.CanExport)
            .WithMessage("At least one permission must be selected.");
    }
        
    }
    }
}