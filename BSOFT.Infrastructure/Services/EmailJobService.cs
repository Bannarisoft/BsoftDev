using BSOFT.Infrastructure.Services;
using Core.Domain.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class EmailJobService : BackgroundService
{
    private readonly EmailService _emailService;
    private readonly ILogger<EmailJobService> _logger;
    private readonly EmailJobSettings _emailJobSettings;
    private readonly EmailSettings _emailSettings;

    public EmailJobService(EmailService emailService, ILogger<EmailJobService> logger, IOptions<EmailJobSettings> emailJobSettings,  IOptions<EmailSettings> emailSettings)
    {
        _emailService = emailService;
        _logger = logger;
        _emailJobSettings = emailJobSettings.Value;
        _emailSettings = emailSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;

                // Execute Daily Jobs
                foreach (var job in _emailJobSettings.DailyJobs)
                {
                    if (now.Hour == job.RunHour && now.Minute == job.RunMinute)
                    {
                        await PerformActionAsync(job.Action);
                    }
                }

                // Execute Weekly Jobs
                foreach (var job in _emailJobSettings.WeeklyJobs)
                {
                    if (now.DayOfWeek.ToString().Equals(job.WeeklyRunDay, StringComparison.OrdinalIgnoreCase) &&
                        now.Hour == job.RunHour && now.Minute == job.RunMinute)
                    {
                        await PerformActionAsync(job.Action);
                    }
                }

                // Execute Monthly Jobs
                foreach (var job in _emailJobSettings.MonthlyJobs)
                {
                    if (now.Day == job.MonthlyRunDay && now.Hour == job.RunHour && now.Minute == job.RunMinute)
                    {
                        await PerformActionAsync(job.Action);
                    }
                }

                // Execute Yearly Jobs
                foreach (var job in _emailJobSettings.YearlyJobs)
                {
                    if (now.Month == job.YearlyRunMonth && now.Day == job.YearlyRunDay &&
                        now.Hour == job.RunHour && now.Minute == job.RunMinute)
                    {
                        await PerformActionAsync(job.Action);
                    }
                }

                // Delay by one minute to avoid multiple executions within the same minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing email jobs.");
            }
        }
    }

    private async Task PerformActionAsync(string action)
    {
        try
        {
            _logger.LogInformation("Executing action: {Action}", action);
            
            switch (action)
            {
                case "DailyReport":
                    var dailyRecipient = _emailSettings.Recipients["DailyReport"];
                    await _emailService.SendEmailAsync(dailyRecipient , "Daily Report", "This is the daily report.");
                    break;
                case "DailySummary":
                    var dailySummaryRecipient = _emailSettings.Recipients["DailySummary"];
                    await _emailService.SendEmailAsync(dailySummaryRecipient, "Daily Summary", "This is the daily summary.");
                    break;

                case "WeeklyReport":
                    var weeklyRecipient = _emailSettings.Recipients["WeeklyReport"];
                    await _emailService.SendEmailAsync(weeklyRecipient, "Weekly Report", "This is the weekly report.");
                    break;

                case "MonthlyNewsletter":
                    var monthlyRecipient = _emailSettings.Recipients["MonthlyNewsletter"];
                    await _emailService.SendEmailAsync(monthlyRecipient, "Monthly Newsletter", "This is the monthly newsletter.");
                    break;

                case "YearlyReview":
                    var yearlyRecipient = _emailSettings.Recipients["YearlyReview"];
                    await _emailService.SendEmailAsync(yearlyRecipient, "Yearly Review", "This is the yearly review.");
                    break;

                default:
                    _logger.LogWarning("Unknown action: {Action}", action);
                    break;
            }

            _logger.LogInformation("Action {Action} completed successfully.", action);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while performing action {Action}.", action);
        }
    }
}
