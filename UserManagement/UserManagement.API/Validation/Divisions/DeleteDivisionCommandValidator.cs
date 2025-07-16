using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDivision;
using Core.Application.Divisions.Commands.DeleteDivision;
using FluentValidation;
using UserManagement.API.Validation.Common;

namespace UserManagement.API.Validation.Divisions
{
    public class DeleteDivisionCommandValidator : AbstractValidator<DeleteDivisionCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IDivisionQueryRepository _divisionQueryRepository;
        public DeleteDivisionCommandValidator(IDivisionQueryRepository divisionQueryRepository)
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _divisionQueryRepository = divisionQueryRepository;

              if (_validationRules == null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }

             foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.Id)
                            .NotEmpty()
                            .WithMessage($"{nameof(DeleteDivisionCommand.Id)} {rule.Error}");
                        break;
                    case "SoftDelete":
                         RuleFor(x => x.Id)
                      .MustAsync(async (Id, cancellation) => !await _divisionQueryRepository.SoftDeleteValidation(Id))
                        .WithMessage($"{rule.Error}");
                        break;
                    default:
                        
                        break;
                }
            }
        }
    }
}