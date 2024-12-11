using  FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Departments.Commands.UpdateDepartment;

namespace BSOFT.Application.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentCommand>
    {
          public UpdateDepartmentValidator()
        {
             RuleFor(v => v.ShortName)
        .NotEmpty().WithMessage("ShortName is required.")
        .MaximumLength(6).WithMessage("ShortName must not exceed 6 characters.");

          RuleFor(v => v.DeptName)
            .NotEmpty().WithMessage("Department Name is required.")
             .MaximumLength(30).WithMessage("Department must not exceed 30 characters.");

         RuleFor(v => v.CoId)
            .NotEmpty().WithMessage("CompanyId is required.");                   

        RuleFor(v => v.IsActive)
            .NotEmpty().WithMessage("IsActive is required.");
               
        }

    }
}