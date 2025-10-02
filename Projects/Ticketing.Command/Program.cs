using MediatR;
using Scalar.AspNetCore;
using Ticketing.Command.Aplication;
using Ticketing.Command.Feature.Apis;

using Ticketing.Command.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.RegisterMinimalApis();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApliactionServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(opt =>
    {
        opt.Title = "Microservice Command con Scalar";
        opt.DarkMode = true;
        opt.Theme = ScalarTheme.BluePlanet;
        opt.DefaultHttpClient = new (ScalarTarget.Http,ScalarClient.Http11);
    });
}

//app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast = Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");

// app.MapPost("/api/ticket", async(
//     IMediator mediator, TicketCreateRequest request, CancellationToken cancellationToken) =>
// {
//     var Command = new TicketCreateCommand(request);
//     var result = await mediator.Send(Command, cancellationToken);

//     return Results.Ok(result);
// }
//  ).WithName("CreateTicket");

app.MapMinimalApisEndpoints();

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
