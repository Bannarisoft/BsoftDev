namespace Core.Application.Item.Templates.SearchTemplates
{
    public sealed class TemplateDto
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = null!;
        public List<TemplateParamDto> Parameters { get; set; } = new();
    }
}
