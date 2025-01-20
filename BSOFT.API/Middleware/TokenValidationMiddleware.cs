using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IUserSession;


namespace BSOFT.API.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        
        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;            
        }

        public async Task Invoke(HttpContext context, IJwtTokenHelper jwtTokenHelper, IUserSessionRepository sessionRepository)
        {
            var path = context.Request.Path.Value;

            // Skip token validation for specific endpoints
            if (path.Equals("/api/auth/login", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/api/auth/register", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
                   // Check for token in the Authorization header
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Authorization token is missing.");
                return;
            }
            try
            {
                // Validate the token
                var principal = jwtTokenHelper.ValidateToken(token);
                var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jti))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid JWT ID.");
                    return;
                }
                
                // Retrieve browser information from User-Agent header
                var browserInfo = context.Request.Headers["User-Agent"].ToString();                                     

                // Check session in the database
                var session = await sessionRepository.GetSessionByJwtIdAsync(jti);
                DateTime utcNow = DateTime.UtcNow;
                // Define the IST timezone
                TimeZoneInfo indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                // Convert UTC to IST
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, indianZone);
                if (session == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Session not found.");
                    return;
                }
                if (session.IsActive==0 || session.ExpiresAt <= indianTime)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Session is invalid or expired.");
                    return;
                }

                // Extract UserId from claims            
                //var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId || c.Type == "nameid");
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
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }

        await _next(context);
        }
    }
}