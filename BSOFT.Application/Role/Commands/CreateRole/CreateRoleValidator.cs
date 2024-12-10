using  FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Role.Commands.CreateRole;

namespace BSOFT.Application.Role.Commands.CreateRole
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
          public CreateRoleValidator()
        {
           RuleFor(v => v.Name)
        .NotEmpty().WithMessage("Name is required.")
        .MaximumLength(100).WithMessage("ShortName must not exceed 6 characters.")
        .Matches("^[A-Za-z]+$").WithMessage("ShortName must contain only alphabetic characters.");

          RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description Name is required.")
             .MaximumLength(30).WithMessage("Description must not exceed 30 characters.");

         RuleFor(v => v.CoId)
            .NotEmpty().WithMessage("CompanyId is required.")
            .GreaterThan(0).WithMessage("CompanyId must be a positive integer.");                   

        RuleFor(v => v.IsActive)
            .NotEmpty().WithMessage("IsActive is required.")
             .InclusiveBetween((byte)0, (byte)1).WithMessage("IsActive must be either 0 or 1.");
               
        }
    }
}