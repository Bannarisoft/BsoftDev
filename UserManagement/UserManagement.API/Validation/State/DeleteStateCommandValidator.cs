using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IState;
using Core.Application.State.Commands.DeleteState;
using FluentValidation;
using Hangfire.States;
using UserManagement.API.Validation.Common;

namespace UserManagement.API.Validation.State
{
    public class DeleteStateCommandValidator : AbstractValidator<DeleteStateCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IStateQueryRepository _stateQueryRepository;
        public DeleteStateCommandValidator( IStateQueryRepository stateQueryRepository)
        {
             _validationRules = ValidationRuleLoader.LoadValidationRules();
            _stateQueryRepository = stateQueryRepository;

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
                            .WithMessage($"{nameof(DeleteStateCommand.Id)} {rule.Error}");
                        break;
                    case "SoftDelete":
                         RuleFor(x => x.Id)
                      .MustAsync(async (Id, cancellation) => !await _stateQueryRepository.SoftDeleteValidation(Id))
                        .WithMessage($"{rule.Error}");
                        break;
                    default:
                        
                        break;
                }
            }
        }
    }
}