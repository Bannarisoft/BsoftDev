namespace BackgroundService.Application.DTO
{
    public class NotificationTargetDto
    {
        public string ChannelName { get; set; } = string.Empty;
        public string RecipientType { get; set; } = string.Empty;
        public string TargetUserIds { get; set; } = string.Empty;
        public string TargetEmailIds { get; set; } = string.Empty;
        public string TargetMobileNumbers { get; set; } = string.Empty;
        public string SubjectTemplate { get; set; } = string.Empty;
        public string BodyTemplate { get; set; } = string.Empty;
        public string FooterTemplate { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = "en";
        public string? TargetCcEmails { get; set; }
        public string? TargetBccEmails { get; set; }
        public int? EventTypeId { get; set; }
        public int? EventRuleId { get; set; }
        public int? ChannelId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset Date { get; set; }

    }
}