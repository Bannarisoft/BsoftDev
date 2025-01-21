using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IUserSession;
using Core.Domain.Entities;
using Microsoft.Extensions.Options;

namespace BSOFT.API.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;
        
        public TokenValidationMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value; 
        }

        public async Task Invoke(HttpContext context, IJwtTokenHelper jwtTokenHelper, IUserSessionRepository sessionRepository)
        {
            var path = context.Request.Path.Value;

            // Skip token validation for specific endpoints
            if (
                path.StartsWith("/api/Auth/")
                )
            {
                await _next(context);
                return;
            }

            // Check for token in the Authorization header
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Authorization token is missing.");
                return;
            }

            try
            {
                // Validate the token
                var principal = jwtTokenHelper.ValidateToken(token);
                var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jti))
                {
                    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Invalid JWT ID.");
                    return;
                }

                // Retrieve browser information from User-Agent header
                var browserInfo = context.Request.Headers["User-Agent"].ToString();

                // Check session in the database
                var session = await sessionRepository.GetSessionByJwtIdAsync(jti);
                DateTime utcNow = DateTime.UtcNow;

                // Define the IST timezone
                TimeZoneInfo indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, indianZone);

                if (session == null)
                {
                    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Session not found.");
                    return;
                }

                if (session.IsActive == 0 || session.ExpiresAt <= indianTime)
                {
                    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Session is invalid or expired.");
                    return;
                }

                // Extract UserId from claims
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    context.Items["UserId"] = int.Parse(userIdClaim.Value); // Attach UserId to HttpContext
                }

                // Retrieve Username
                var userNameClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name);
                if (userNameClaim != null)
                {
                    context.Items["UserName"] = userNameClaim.Value; // Attach Username to HttpContext
                }

                // Update the session's last activity
                session.LastActivity = DateTime.UtcNow;
                session.BrowserInfo = browserInfo;
                await sessionRepository.UpdateSessionAsync(session);

                // Set user context for the current request
                context.User = principal;
            }
            catch (Exception ex)
            {
                await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, $"Invalid token: {ex.Message}");
                return;
            }

            await _next(context);
        }

        private async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
