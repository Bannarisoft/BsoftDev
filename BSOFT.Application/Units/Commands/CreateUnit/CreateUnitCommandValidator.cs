using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using BSOFT.Application.Units.Commands.CreateUnit;

namespace BSOFT.Application.Units.Commands.CreateUnit
{
    public class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
    {
        public CreateUnitCommandValidator()
        {
        //      RuleFor(v => v.UnitName)
        // .NotEmpty().WithMessage("UnitName is required.")
        // .MaximumLength(50).WithMessage("UnitName must not exceed 50 characters.");

        //  RuleFor(v => v.ShortName)
        //     .NotEmpty().WithMessage("ShortName is required.")
        //     .MaximumLength(6).WithMessage("ShortName must not exceed 6 characters.");

         
        // RuleFor(v => v.UnitHeadName)
        //     .NotEmpty().WithMessage("UnitHeadName is required.")
        //     .MaximumLength(20).WithMessage("UnitHeadName must not exceed 20 characters.");


   

               
        }
        
  
    }
}