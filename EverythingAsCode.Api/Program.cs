using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>()
    {
        ["SomeKey"] = "SomeValue"
    })
    .AddEnvironmentVariables()
    .Build();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
var client = new EventGridPublisherClient(
    new Uri("https://everythingascodetopic.westeurope-1.eventgrid.azure.net/api/events"),
    new AzureKeyCredential(configuration["EVENTGRID_TOPIC_KEY"]));
app.MapPost("/publish", async () =>
{
    var eventGridEvent = new EventGridEvent("ThingHappened", "Thing", "ThingData", new { A = 1});
    await client.SendEventAsync(eventGridEvent);
});
var events = new List<string>();
app.MapPost("/listen",  context =>
{
    var rawRequestBody = new StreamReader(context.Request.Body).ReadToEnd();
    events.Add(rawRequestBody);
    return Task.CompletedTask;
});
app.MapGet("/events", () => events.OrderDescending().ToList());

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
