// FixedAssetManagement.Tests/UnitTests/Pipeline/ExceptionHandlingMiddlewareTests.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using System.Text;
using FluentValidation;

// === Adjust these namespaces/types to match your project ===
// If you already have these exceptions & DTO in your Core project, import them instead of the stubs below.
namespace FixedAssetManagement.Tests.UnitTests.Pipeline
{
    // ---- Stubs (replace with your real ones if available) ----
    public class EntityNotFoundException : Exception { public EntityNotFoundException(string m) : base(m) { } }
    public class EntityAlreadyExistsException : Exception { public EntityAlreadyExistsException(string m) : base(m) { } }

    public class ApiResponseDTO<T>
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    /// <summary>
    /// Minimal copy of your ExceptionHandlingMiddleware contract.
    /// If your real middleware lives in another project, remove this class and
    /// just `using` the namespace where it’s defined.
    /// </summary>
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (EntityNotFoundException ex)
            {
                await Write(ctx, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (EntityAlreadyExistsException ex)
            {
                await Write(ctx, StatusCodes.Status409Conflict, ex.Message);
            }
            catch (ValidationException ex)
            {
                await Write(ctx, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await Write(ctx, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task Write(HttpContext ctx, int status, string message)
        {
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/json";
            var payload = new ApiResponseDTO<object>
            {
                StatusCode = status,
                IsSuccess  = false,
                Message    = message,
                Data       = null
            };
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }

    [TestClass]
    public class ExceptionHandlingMiddlewareTests
    {
        private static HttpContext NewHttp()
        {
            var ctx = new DefaultHttpContext();
            ctx.Response.Body = new MemoryStream(); // capture output
            return ctx;
        }

        private static async Task<(HttpContext Ctx, ApiResponseDTO<object>? Body)>
            InvokeAndReadAsync(Exception? exToThrow)
        {
            // Arrange: next delegate either throws or completes
            RequestDelegate next = _ => exToThrow is null
                ? Task.CompletedTask
                : Task.FromException(exToThrow);

            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>(MockBehavior.Loose).Object;
            var sut = new ExceptionHandlingMiddleware(next, logger);
            var ctx = NewHttp();

            // Act
            await sut.InvokeAsync(ctx);

            // Read body if any
            ctx.Response.Body.Position = 0;
            if (ctx.Response.Body.Length == 0)
                return (ctx, null);

            var json = await new StreamReader(ctx.Response.Body, Encoding.UTF8).ReadToEndAsync();
            var body = JsonSerializer.Deserialize<ApiResponseDTO<object>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return (ctx, body);
        }

        [TestMethod]
        public async Task When_EntityNotFound_Returns_404_With_ApiResponse()
        {
            var (ctx, body) = await InvokeAndReadAsync(new EntityNotFoundException("not found"));

            Assert.AreEqual(StatusCodes.Status404NotFound, ctx.Response.StatusCode);
            Assert.IsNotNull(body);
            Assert.AreEqual(StatusCodes.Status404NotFound, body!.StatusCode);
            Assert.IsFalse(body.IsSuccess);
            Assert.AreEqual("not found", body.Message);
            Assert.AreEqual("application/json", ctx.Response.ContentType);
        }

        [TestMethod]
        public async Task When_EntityAlreadyExists_Returns_409_With_ApiResponse()
        {
            var (ctx, body) = await InvokeAndReadAsync(new EntityAlreadyExistsException("exists"));

            Assert.AreEqual(StatusCodes.Status409Conflict, ctx.Response.StatusCode);
            Assert.IsNotNull(body);
            Assert.AreEqual(StatusCodes.Status409Conflict, body!.StatusCode);
            Assert.IsFalse(body.IsSuccess);
            Assert.AreEqual("exists", body.Message);
        }

        [TestMethod]
        public async Task When_FluentValidationException_Returns_400_With_ApiResponse()
        {
            var (ctx, body) = await InvokeAndReadAsync(new ValidationException("bad input"));

            Assert.AreEqual(StatusCodes.Status400BadRequest, ctx.Response.StatusCode);
            Assert.IsNotNull(body);
            Assert.AreEqual(StatusCodes.Status400BadRequest, body!.StatusCode);
            Assert.IsFalse(body.IsSuccess);
            Assert.AreEqual("bad input", body.Message);
        }

        [TestMethod]
        public async Task When_UnexpectedException_Returns_500_Generic_Message()
        {
            var (ctx, body) = await InvokeAndReadAsync(new InvalidOperationException("boom"));

            Assert.AreEqual(StatusCodes.Status500InternalServerError, ctx.Response.StatusCode);
            Assert.IsNotNull(body);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, body!.StatusCode);
            Assert.IsFalse(body.IsSuccess);
            Assert.AreEqual("An unexpected error occurred.", body.Message);
        }

        [TestMethod]
        public async Task When_No_Exception_Passes_Through_Without_Writing_Body()
        {
            var (ctx, body) = await InvokeAndReadAsync(null);

            // Default status is 200 and no body should be written by middleware
            Assert.AreEqual(StatusCodes.Status200OK, ctx.Response.StatusCode);
            Assert.IsNull(body);
            Assert.AreEqual(0, ctx.Response.Body.Length);
        }
    }
}
