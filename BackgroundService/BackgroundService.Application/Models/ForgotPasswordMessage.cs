using System.Text.Json.Serialization;
using Notification.Domain.Enums;

namespace BackgroundService.Application.Models
{
    public class ForgotPasswordMessage
{
    [JsonPropertyName("Provider")]
    public string Provider { get; set; } = "Gmail";

    [JsonPropertyName("ToEmail")]
    public string ToEmail { get; set; } = string.Empty;

    [JsonPropertyName("Subject")]
    public string Subject { get; set; } = string.Empty;

    [JsonPropertyName("HtmlContent")]
    public string HtmlContent { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    
    }
}