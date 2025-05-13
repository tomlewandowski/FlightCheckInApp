using FlightCheckIn.Api;
using FlightCheckIn.Domain;
using FlightCheckIn.Domain.Policies;
using FlightCheckIn.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddSingleton<IFlightRepository, InMemoryFlightRepository>();
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddSingleton<IBaggagePolicy, StandardBaggagePolicy>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightCheckIn API V1");
});
app.UseMiddleware<ExceptionMappingMiddleware>();
app.MapFlightEndpoints();
app.Run();
