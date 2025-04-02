using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Infrastructure.Configurations
{
    public class SmsSettings
    {
        public string? Provider { get; set; }
        public string? ApiKey { get; set; }
    }
}