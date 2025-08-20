using MediatR;

namespace Core.Application.Item.Templates.SearchTemplates
{
    public sealed class SearchTemplatesQuery : IRequest<List<TemplateDto>>
    {
        public string? Term { get; init; }
        public int Take { get; init; } = 20;
    }
}