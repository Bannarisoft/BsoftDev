using Core.Application.Common.Interfaces.Item.Templates;
using Core.Domain.Entities.item.ItemDetail.Templates;
using Core.Domain.Entities.Item.ItemDetail.Templates;
using MediatR;

namespace Core.Application.Item.Templates.CreateTemplate
{
    public sealed class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, int>
    {
        private readonly ITemplateRepository _repo;
        public CreateTemplateCommandHandler(ITemplateRepository repo) => _repo = repo;

        public async Task<int> Handle(CreateTemplateCommand req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.TemplateName))
                throw new ArgumentException("Template name is required.");

            if (await _repo.ExistsByNameAsync(req.TemplateName.Trim(), ct))
                throw new InvalidOperationException("A template with the same name already exists.");

            var tpl = new InspectionTemplate
            {
                TemplateName = req.TemplateName.Trim()
            };

            foreach (var p in req.Parameters ?? new())
            {
                tpl.Parameters.Add(new InspectionParameter
                {
                    Parameter = p.Parameter.Trim(),
                    AcceptanceCriteriaValue = p.AcceptanceCriteriaValue?.Trim(),
                    Numeric = p.Numeric,
                    MinimumValue = p.MinimumValue,
                    MaximumValue = p.MaximumValue
                });
            }

            return await _repo.CreateAsync(tpl, ct);
        }
    }
}
