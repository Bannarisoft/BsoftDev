using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.ICountry;
using Core.Application.Country.Commands.DeleteCountry;
using FluentValidation;
using UserManagement.API.Validation.Common;

namespace UserManagement.API.Validation.Country
{
    public class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly ICountryQueryRepository _countryQueryRepository;
        public DeleteCountryCommandValidator(ICountryQueryRepository countryQueryRepository)
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _countryQueryRepository = countryQueryRepository;
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
                            .WithMessage($"{nameof(DeleteCountryCommand.Id)} {rule.Error}");
                        break;
                    case "SoftDelete":
                         RuleFor(x => x.Id)
                      .MustAsync(async (Id, cancellation) => !await _countryQueryRepository.SoftDeleteValidation(Id))
                        .WithMessage($"{rule.Error}");
                        break;
                    default:
                        
                        break;
                }
            }
        }
    }
}