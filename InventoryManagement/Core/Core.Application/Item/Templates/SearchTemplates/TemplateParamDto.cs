namespace Core.Application.Item.Templates.SearchTemplates
{
    public sealed class TemplateParamDto
    {
        public string Parameter { get; set; } = null!;
        public string? AcceptanceCriteriaValue { get; set; }
        public bool Numeric { get; set; }
        public decimal? MinimumValue { get; set; }
        public decimal? MaximumValue { get; set; }
    }   
}
