using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Infrastructure.Services
{
    public static class TimeZoneService
    {
        private static readonly string _systemTimeZoneId;

       

        public static DateTimeOffset ConvertUtcToTimeZone(DateTimeOffset utcDateTime, string timeZoneId)
        {   try
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTime(utcDateTime, timeZone);
        }
        catch (TimeZoneNotFoundException)    
        {
            throw new ArgumentException($"Invalid TimeZoneId: {timeZoneId}");
        }
        }

        public static DateTimeOffset GetCurrentTime(string timeZoneId)
        {
             return ConvertUtcToTimeZone(DateTimeOffset.UtcNow, timeZoneId);
        }

        // public DateTime ConvertUtcToTimeZone(DateTime utcDateTime, string timeZoneId)
        // {
        //     try
        //     {
        //         var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        //         return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
        //     }
        //     catch (TimeZoneNotFoundException)
        //     {
        //         throw new ArgumentException($"Invalid TimeZoneId: {timeZoneId}");
        //     }
        // }
        // public DateTime GetCurrentTime(string timeZoneId)
        // {
        //     return ConvertUtcToTimeZone(DateTime.UtcNow, timeZoneId);
        // }



        public static string GetSystemTimeZone()
        {
            return _systemTimeZoneId;
        }

    }
}