using BackgroundService.Application.DTO;
using BackgroundService.Application.Interfaces.Notification;

namespace BackgroundService.Application.Notification
{
    public class NotificationResolverHandler
    {
        private readonly INotificationUserResolver _resolver;

        public NotificationResolverHandler(INotificationUserResolver resolver)
        {
            _resolver = resolver;
        }

         public async Task<(List<string> ToEmails, List<string> CcEmails, List<string> BccEmails, List<string> SmsNumbers, List<int> InAppUserIds, string Subject, string header, string Body, string Footer, string LangCode,int? EventTypeId,int? EventRuleId, int? ChannelId)>
            ResolveNotificationTemplatesAsync(int unitId, string module, int eventTypeId)
        {
            var targets = await _resolver.GetNotificationTargetsAsync(unitId, module, eventTypeId);

            var smsNumbers = targets
                .Where(t => t.ChannelName == "SMS")
                .SelectMany(t => (t.TargetMobileNumbers ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(n => n.Trim())
                .Distinct()
                .ToList();

            var inAppUserIds = targets
                .Where(t => t.ChannelName == "InApp")
                .SelectMany(t => (t.TargetUserIds ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(id => int.TryParse(id.Trim(), out var val) ? val : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var emailTargets = targets.Where(t => t.ChannelName == "Email").ToList();

            var toEmails = emailTargets.SelectMany(t => (t.TargetEmailIds ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)).Select(e => e.Trim()).Distinct().ToList();
            var ccEmails = emailTargets.SelectMany(t => (t.TargetCcEmails ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)).Select(e => e.Trim()).Distinct().ToList();
            var bccEmails = emailTargets.SelectMany(t => (t.TargetBccEmails ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)).Select(e => e.Trim()).Distinct().ToList();

            var subject = emailTargets.FirstOrDefault()?.SubjectTemplate ?? string.Empty;
            var body = emailTargets.FirstOrDefault()?.BodyTemplate ?? string.Empty;
            var header = emailTargets.FirstOrDefault()?.HeaderTemplate ?? string.Empty;
            var footer = emailTargets.FirstOrDefault()?.FooterTemplate ?? string.Empty;
            var langCode = targets.FirstOrDefault()?.LanguageCode ?? "en";

            return (toEmails, ccEmails, bccEmails, smsNumbers, inAppUserIds, subject, header,body, footer, langCode,eventTypeId,targets.FirstOrDefault()?.EventRuleId,targets.FirstOrDefault()?.ChannelId);    
        }

        // Used by ResolveNotificationChannelsConsumer to decide which channels to publish
        public async Task<List<string>> ResolveNotificationChannelsAsync(int unitId, string module, int eventTypeId)
        {
            var targets = await _resolver.GetNotificationTargetsAsync(unitId, module, eventTypeId);

            return targets
                .Select(t => t.ChannelName)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}