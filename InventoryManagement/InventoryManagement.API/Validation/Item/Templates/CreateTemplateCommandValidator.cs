// ---------------------------------------------------------
// 2) Parent command validator
// ---------------------------------------------------------
using System.Linq;
using FluentValidation;
using Core.Application.Item.Templates.CreateTemplate;
using Core.Application.Item.Templates.SearchTemplates;

public sealed class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
{
    public CreateTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Parameters)
            .NotNull().WithMessage("Parameters collection is required.")
            .Must(p => p.Count > 0).WithMessage("At least one parameter is required.");

        // apply the row validator to each item
        RuleForEach(x => x.Parameters)
            .SetValidator(new TemplateParamDtoValidator());

        // Optional: prevent duplicate parameter names (case-insensitive)
        RuleFor(x => x.Parameters)
            .Must(list => list
                .Select(p => (p.Parameter ?? string.Empty).Trim().ToLowerInvariant())
                .Distinct().Count() == list.Count)
            .WithMessage("Duplicate parameter names are not allowed.");
    }
}
