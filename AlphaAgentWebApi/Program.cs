using AlphaAgentWebApi.Interfaces;
using AlphaAgentWebApi.Factories;
using AlphaAgentWebApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<AgentConfiguration>(builder.Configuration.GetSection("AgentConfiguration"));
builder.Services.AddSingleton<IAgentFactory, AgentFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
