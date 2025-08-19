// ---------------------------------------------------------
// 1) Child validator for each parameter row
// ---------------------------------------------------------
using System.Globalization;
using FluentValidation;
using Core.Application.Item.Templates.SearchTemplates;

public sealed class TemplateParamDtoValidator : AbstractValidator<TemplateParamDto>
{
    public TemplateParamDtoValidator()
    {
        RuleFor(x => x.Parameter).NotEmpty();

        When(x => x.Numeric, () =>
        {
            RuleFor(x => x.MinimumValue)
                .NotNull().WithMessage("Minimum Value is required for numeric parameters.");

            RuleFor(x => x.MaximumValue)
                .NotNull().WithMessage("Maximum Value is required for numeric parameters.");

            RuleFor(x => x)
                .Must(x => x.MinimumValue <= x.MaximumValue)
                .WithMessage("Minimum Value must be less than or equal to Maximum Value.");

            RuleFor(x => x.AcceptanceCriteriaValue)
                .NotEmpty().WithMessage("Acceptance Criteria Value is required for numeric parameters.")
                .Must(v => decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
                .WithMessage("Acceptance Criteria Value must be numeric.");

            RuleFor(x => x)
                .Must(x =>
                {
                    if (!decimal.TryParse(x.AcceptanceCriteriaValue, NumberStyles.Number,
                        CultureInfo.InvariantCulture, out var acc)) return false;

                    return x.MinimumValue.HasValue && x.MaximumValue.HasValue &&
                           acc >= x.MinimumValue.Value && acc <= x.MaximumValue.Value;
                })
                .WithMessage("Acceptance Criteria must be within Minimum and Maximum Value.");
        });

        When(x => !x.Numeric, () =>
        {
            RuleFor(x => x.AcceptanceCriteriaValue)
                .NotEmpty().WithMessage("Acceptance Criteria Value is required.")
                .MaximumLength(100)
                .Matches(@"^[\p{L}\p{N}\s\.\-_]+$")
                .WithMessage("Only letters, numbers, space, dot, hyphen and underscore are allowed.");

            RuleFor(x => x.MinimumValue)
                .Null().WithMessage("Minimum Value must be empty when Numeric is unticked.");

            RuleFor(x => x.MaximumValue)
                .Null().WithMessage("Maximum Value must be empty when Numeric is unticked.");
        });
    }
}
