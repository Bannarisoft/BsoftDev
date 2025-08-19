using Core.Application.Item.Templates.SearchTemplates;
using MediatR;

namespace Core.Application.Item.Templates.CreateTemplate
{
    public sealed class CreateTemplateCommand : IRequest<int>
    {
        public string TemplateName { get; init; } = null!;
        public List<TemplateParamDto> Parameters { get; init; } = new();
    }   
}
