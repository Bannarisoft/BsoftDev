namespace BackgroundService.Application.DTO
{
    public class NotificationTargetDto
    {
        public string? ChannelName { get; set; }
        public string? TargetEmailIds { get; set; }
        public string? TargetCcEmails { get; set; }
        public string? TargetBccEmails { get; set; }
        public string? TargetMobileNumbers { get; set; }
        public string? TargetUserIds { get; set; }

        public string? SubjectTemplate { get; set; }
        public string? BodyTemplate { get; set; }
        public string? FooterTemplate { get; set; }
        public string? LanguageCode { get; set; }
    }
}