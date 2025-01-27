using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BSOFT.Infrastructure.Services
{
   public class SmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;

        public SmsService(IConfiguration configuration)
        {
            /* // Load Twilio settings from appsettings.json or environment variables
            _accountSid = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            _fromPhoneNumber = configuration["Twilio:FromPhoneNumber"];

            if (string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken) || string.IsNullOrEmpty(_fromPhoneNumber))
            {
                throw new InvalidOperationException("Twilio configuration is missing.");
            }

            // Initialize Twilio client
            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task<bool> SendSmsAsync(string to, string message)
        {
            try
            {
                var result = await MessageResource.CreateAsync(
                    to: new PhoneNumber(to),
                    from: new PhoneNumber(_fromPhoneNumber),
                    body: message
                );

                Console.WriteLine($"SMS sent successfully. SID: {result.Sid}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send SMS. Error: {ex.Message}");
                return false;
            }
        }*/
    }
}
}