using Core.Application;
using BSOFT.Infrastructure;
using BSOFT.API.Validation.Common;
using MediatR;
using Core.Application.State.Commands.CreateState;


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

var app = builder.Build();
 
 // Map endpoint to handle CreateStateCommand
app.MapPost("/state", async (
    CreateStateCommand request,
    IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var result = await mediator.Send(request, cancellationToken);

    if (!result.IsSuccess)
    {
        return Results.BadRequest(result.ErrorMessage);
    }

    return Results.Created($"/states/{result.Data.Id}", result.Data);
});

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
