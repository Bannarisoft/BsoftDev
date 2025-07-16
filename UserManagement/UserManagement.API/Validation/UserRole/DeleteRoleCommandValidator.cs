using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserRole;
using Core.Application.UserRole.Commands.DeleteRole;
using FluentValidation;
using UserManagement.API.Validation.Common;

namespace UserManagement.API.Validation.UserRole
{
    public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
    
        private readonly List<ValidationRule> _validationRules;
        private readonly IUserRoleQueryRepository _userRoleQueryRepository;
        public DeleteRoleCommandValidator(IUserRoleQueryRepository userRoleQueryRepository)
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _userRoleQueryRepository = userRoleQueryRepository;
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
                            .WithMessage($"{nameof(DeleteRoleCommand.Id)} {rule.Error}");
                        break;
                    case "SoftDelete":
                         RuleFor(x => x.Id)
                      .MustAsync(async (Id, cancellation) => !await _userRoleQueryRepository.SoftDeleteValidation(Id))
                        .WithMessage($"{rule.Error}");
                        break;
                    default:
                        
                        break;
                }
            }
        }
    }
}