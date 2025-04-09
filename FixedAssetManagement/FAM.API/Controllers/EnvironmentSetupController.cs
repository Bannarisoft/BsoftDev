using System.Text.Json;
using Core.Application.Common.EnvironmentSetup;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers
{
    [Route("api/security")]
    [ApiController]
    public class EnvironmentSetupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public EnvironmentSetupController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }


          [HttpPost("set-connection")]
        public async Task<IActionResult> SetConnectionString([FromBody] ConnectionStringRequest request)
        {
            // Encrypt the password using Mediator (CQRS)
            var encryptedPassword = await _mediator.Send(new EncryptPasswordCommand(request.Password));
            var encryptedServer = await _mediator.Send(new EncryptPasswordCommand(request.Server));
            var encryptedUserId = await _mediator.Send(new EncryptPasswordCommand(request.UserId));

            Environment.SetEnvironmentVariable("DATABASE_SERVER", encryptedServer, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("DATABASE_USERID", encryptedUserId, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("DATABASE_PASSWORD", encryptedPassword, EnvironmentVariableTarget.Process);
            
//            string connectionString = $"Server={request.Server};Database=FixedAsset;User Id={request.UserId};Password={encryptedPassword};Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true;";

                // Load `appSettingsPath` from Environment Variable
            var appSettingsPath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(appSettingsPath))
            {
                // If not set, fallback to default path
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.{environment}.json");

                // Set `APP_SETTINGS_PATH` Environment Variable
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", appSettingsPath, EnvironmentVariableTarget.Process);
            }

            if (!System.IO.File.Exists($"appsettings.{appSettingsPath}.json"))
            {
                return NotFound(new { Message = $"Configuration file '{appSettingsPath}' not found." });
            }

            var json = System.IO.File.ReadAllText($"appsettings.{appSettingsPath}.json");
            var jsonDocument = JsonDocument.Parse(json);

            var jsonObject = jsonDocument.RootElement.Clone();
            var jsonObjectString = jsonObject.ToString();

            // Update `Encryption` section using key names
            jsonObjectString = jsonObjectString
                .Replace("{DefaultEncryptedPassword}", encryptedPassword)
                .Replace("{DefaultEncryptedServer}", encryptedServer)
                .Replace("{DefaultEncryptedUserId}", encryptedUserId);

            System.IO.File.WriteAllText($"appsettings.{appSettingsPath}.json", jsonObjectString);

            return Ok(new
            {
                Message = "Connection string and encryption details updated successfully.",
                EncryptedServer = encryptedServer,
                EncryptedUserId = encryptedUserId,
                EncryptedPassword = encryptedPassword,
                ConfigFilePath = $"appsettings.{appSettingsPath}.json"
            });
        }
    }

    public class ConnectionStringRequest
    {
        public string? Server { get; set; }
        public string? UserId { get; set; }
        public string? Password { get; set; }    
    }
}