using Core.Application;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging to MongoDB and console
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console() // Log to console for debugging
    .WriteTo.MongoDB("mongodb://192.168.1.126:27017/Bannari") // MongoDB connection string (adjust as needed)
    .WriteTo.MongoDB("mongodb://192.168.1.126:27017/Bannari", collectionName: "ApplicationLogs", restrictedToMinimumLevel: LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging in the app

//Add layer dependency 
builder.Services.AddApplicationServices();
// builder.Services.AddInfrastructureServices(builder.Configuration,builder.Services);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); 

}

app.UseHttpsRedirection();
app.UseRouting(); // Enable routing
// Enable CORS
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllers();
app.Run();
