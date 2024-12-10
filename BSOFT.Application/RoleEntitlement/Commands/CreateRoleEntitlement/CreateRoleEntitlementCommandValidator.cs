using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.RoleEntitlement.Commands.CreateRoleEntitlement
{
    public class CreateRoleEntitlementCommandValidator : AbstractValidator<CreateRoleEntitlementDto>
    {
        public CreateRoleEntitlementCommandValidator()
        {
            RuleFor(x => x.RoleName).NotEmpty().WithMessage("Role Name is required.");
            RuleFor(x => x.MenuPermissions).NotEmpty().WithMessage("Menu Mapping is required.");
            RuleForEach(x => x.MenuPermissions).ChildRules(menu =>
        {
            menu.RuleFor(m => m.MenuName).NotEmpty().WithMessage("Menu Name is required.");
            menu.RuleFor(m => m.CanView)
                .Equal(true)
                .When(m => !m.CanAdd && !m.CanUpdate && !m.CanDelete && !m.CanExport && !m.CanApprove)
                .WithMessage("At least one permission must be selected.");
        });
    }

        
    }
}