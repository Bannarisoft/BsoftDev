namespace BackgroundService.Application.DTO
{
    public class NotificationTargetDto
    {      
        public string ChannelName { get; set; } = string.Empty; // Email, InApp, etc.
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
    
    }
}