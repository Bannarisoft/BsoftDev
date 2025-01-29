using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class CompanySettings : BaseEntity
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int PasswordHistoryCount { get; set; }
        public int SessionTimeout { get; set; }
        public int FailedLoginAttempts { get; set; }
        public int AutoReleaseTime { get; set; }
        public int PasswordExpiryDays { get; set; }
        public int PasswordExpiryAlert { get; set; }
        public byte TwoFactorAuth { get; set; }
        public int MaxConcurrentLogins { get; set; }
        public int ForgotPasswordCodeExpiry { get; set; }
        public byte CaptchaOnLogin { get; set; }
        public int Currency { get; set; }
        public int Language { get; set; }
        public int TimeZone { get; set; }
        public int FinancialYear { get; set; }
        public byte IsActive { get; set; }
    }
}