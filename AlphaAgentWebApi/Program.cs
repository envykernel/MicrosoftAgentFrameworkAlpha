using AlphaAgentWebApi.Interfaces;
using AlphaAgentWebApi.Configuration;
using AlphaAgentWebApi.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<AgentConfiguration>(builder.Configuration.GetSection("AgentConfiguration"));

// Register MongoDB services
builder.Services.AddSingleton<MongoClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<AgentConfiguration>>().Value;
    return new MongoClient(config.MongoDbConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoClient = sp.GetRequiredService<MongoClient>();
    var config = sp.GetRequiredService<IOptions<AgentConfiguration>>().Value;
    return mongoClient.GetDatabase(config.MongoDbDatabaseName);
});

builder.Services.AddSingleton<IAgentProvider, AgentProvider>();
builder.Services.AddScoped<IGeographyAgentService, GeographyAgentService>();
builder.Services.AddScoped<IMathAgentService, MathAgentService>();
builder.Services.AddScoped<IOrchestratorAgentService, OrchestratorAgentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
