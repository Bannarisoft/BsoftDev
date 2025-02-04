using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Language.Commands.CreateLanguage;
using FluentValidation;
using BSOFT.API.Validation.Common;
using Core.Domain.Entities;

namespace BSOFT.API.Validation.Languages
{
    public class CreateLanguageCommandValidator : AbstractValidator<CreateLanguageCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        public CreateLanguageCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            var nameMaxLength = maxLengthProvider.GetMaxLength<Language>("Name") ?? 50;
            var codeMaxLength = maxLengthProvider.GetMaxLength<Language>("Code") ?? 10;

            _validationRules = ValidationRuleLoader.LoadValidationRules();
            if (_validationRules == null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }
            foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.Name)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateLanguageCommand.Name)} {rule.Error}");
                        RuleFor(x => x.Code)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateLanguageCommand.Code)} {rule.Error}");
                        break;
                    case "MaxLength":
                        RuleFor(x => x.Name)
                            .MaximumLength(nameMaxLength)
                            .WithMessage($"{nameof(CreateLanguageCommand.Name)} {rule.Error} {nameMaxLength}");
                        RuleFor(x => x.Code)
                            .MaximumLength(codeMaxLength)
                            .WithMessage($"{nameof(CreateLanguageCommand.Code)} {rule.Error} {codeMaxLength}");
                        break;
                }
            }
        }
    }
}