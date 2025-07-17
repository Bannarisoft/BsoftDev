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

        public async Task<(
            List<string> ToEmails,
            List<string> CcEmails,
            List<string> BccEmails,
            List<string> smsNumbers,
            List<int> inAppUserIds,
            string Subject,
            string Body,
            string Footer,string LangCode)> ResolveEmailChannelAsync(int unitId, string module, int eventTypeId)
        {
            var targets = await _resolver.GetNotificationTargetsAsync(unitId, module, eventTypeId);

            var mobileNumbers  = targets
                .Where(t => t.ChannelName == "SMS")
                .SelectMany(t => t.TargetMobileNumbers?.Split(',') ?? [])
                .Select(n => n.Trim())
                .Distinct()
                .ToList();

            var inAppUserIds = targets
                .Where(t => t.ChannelName == "InApp")
                .SelectMany(t => t.TargetUserIds?.Split(',') ?? [])
                .Select(id => int.TryParse(id.Trim(), out var val) ? val : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();


            var emailTargets = targets.Where(t => t.ChannelName == "Email").ToList();

            var toEmails = emailTargets
                .SelectMany(t => t.TargetEmailIds?.Split(',') ?? [])
                .Select(e => e.Trim())
                .Distinct()
                .ToList();

            var ccEmails = emailTargets
                .SelectMany(t => t.TargetCcEmails?.Split(',') ?? [])
                .Select(e => e.Trim())
                .Distinct()
                .ToList();

            var bccEmails = emailTargets
                .SelectMany(t => t.TargetBccEmails?.Split(',') ?? [])
                .Select(e => e.Trim())
                .Distinct()
                .ToList();

            var subject = targets.FirstOrDefault(t => t.ChannelName == "Email")?.SubjectTemplate;
            var body = targets.FirstOrDefault(t => t.ChannelName == "Email")?.BodyTemplate;
            var footer = targets.FirstOrDefault(t => t.ChannelName == "Email")?.FooterTemplate;
            var langCode = targets.FirstOrDefault()?.LanguageCode ?? "en";

            //return (toEmails, ccEmails, bccEmails, subject, body, footer, langCode)
            return (toEmails, ccEmails, bccEmails, mobileNumbers , inAppUserIds, subject, body, footer, langCode);
        }
    }
}
