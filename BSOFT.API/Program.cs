using Core.Application;
using Core.Domain.Entities;
using BSOFT.Infrastructure;
using BSOFT.API.Validation.Common;
using BSOFT.API.GlobalException;



var builder = WebApplication.CreateBuilder(args);

// Add validation services
var validationService = new ValidationService();
validationService.AddValidationServices(builder.Services);

//Add layer dependency 
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration,builder.Services);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
   
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


var app = builder.Build();
    

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

 app.UseExceptionHandler();

app.MapControllers();

app.Run();
