using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Departments.Commands.CreateDepartment;

namespace Core.Application.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentValidator()
        {
             RuleFor(v => v.ShortName)
        .NotEmpty().WithMessage("ShortName is required.")
        .MaximumLength(6).WithMessage("ShortName must not exceed 6 characters.");

          RuleFor(v => v.DeptName)
            .NotEmpty().WithMessage("Department Name is required.")
             .MaximumLength(30).WithMessage("Department must not exceed 30 characters.");

         RuleFor(v => v.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required.");                   

               
        }
        
  
    }
}