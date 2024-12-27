using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement
{
    public class CreateRoleEntitlementCommandValidator : AbstractValidator<CreateRoleEntitlementCommand>
    {
            public CreateRoleEntitlementCommandValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("RoleName is required.");

        RuleForEach(x => x.ModuleMenus).ChildRules(module =>
        {
            module.RuleFor(m => m.ModuleId)
                .GreaterThan(0).WithMessage("Module ID must be greater than 0.");

            module.RuleForEach(m => m.Menus).ChildRules(menu =>
            {
                menu.RuleFor(me => me.MenuId)
                    .GreaterThan(0).WithMessage("Menu ID must be greater than 0.");
            });
        });
    }
    }
}