using Core.Application.Common.Interfaces.Item.Templates;
using MediatR;

namespace Core.Application.Item.Templates.SearchTemplates
{
    public sealed class SearchTemplatesQueryHandler : IRequestHandler<SearchTemplatesQuery, List<TemplateDto>>
    {
        private readonly ITemplateRepository _repo;
        public SearchTemplatesQueryHandler(ITemplateRepository repo) => _repo = repo;

        public async Task<List<TemplateDto>> Handle(SearchTemplatesQuery req, CancellationToken ct)
        {
            var list = await _repo.SearchAsync(req.Term, req.Take, ct);
            return list.Select(t => new TemplateDto
            {
                Id = t.Id,
                TemplateName = t.TemplateName,
                Parameters = t.Parameters
                    //.OrderBy(p => p.SortOrder) // if BaseEntity has SortOrder; else remove
                    .Select(p => new TemplateParamDto
                    {
                        Parameter = p.Parameter,
                        AcceptanceCriteriaValue = p.AcceptanceCriteriaValue,
                        Numeric = p.Numeric,
                        MinimumValue = p.MinimumValue,
                        MaximumValue = p.MaximumValue
                    }).ToList()
            }).ToList();
        }
    }
}
