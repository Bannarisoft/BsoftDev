using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Utilities
{public static class ForgotPasswordCache
{
    public static readonly Dictionary<string, VerificationCodeDetails> CodeStorage = new();

    // Method to remove verification code
    public static void RemoveVerificationCode(string username)
    {
        if (CodeStorage.ContainsKey(username))
        {
            CodeStorage.Remove(username);
            Console.WriteLine($"Verification code for user {username} has been removed.");
        }
    }
}

// Verification code details class
public class VerificationCodeDetails
{
    public string? Code { get; set; }
    public DateTime ExpiryTime { get; set; }
}

}