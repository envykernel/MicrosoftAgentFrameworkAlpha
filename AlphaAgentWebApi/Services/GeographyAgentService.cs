using AlphaAgentWebApi.Interfaces;
using AlphaAgentWebApi.Models;
using AlphaAgentWebApi.Configuration;
using AlphaAgentWebApi.Constants;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AlphaAgentWebApi.Services;

public class GeographyAgentService : IGeographyAgentService
{
    private readonly AIAgent _geographyAgent;

    public GeographyAgentService(IAgentFactory agentFactory, IOptions<AgentConfiguration> agentConfig)
    {
        var config = agentConfig.Value;
        var geographyOptions = new ChatClientAgentOptions
        {
            Instructions = config.Agents[AgentNames.GeographyAgent].Instructions,
            Name = config.Agents[AgentNames.GeographyAgent].Name,
            ChatOptions = new()
            {
                ResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat.ForJsonSchema<GeographyResponse>()
            }
        };
        
        if (config.Agents.TryGetValue(AgentNames.GeographyAgent, out var geoAgentSettings))
        {
            _geographyAgent = agentFactory.CreateAgent(geographyOptions);
        }
        else
        {
            throw new InvalidOperationException($"{AgentNames.GeographyAgent} not found in configuration");
        }
    }

    public async Task<GeographyResponse> AskGeographyAsync(string question)
    {
        var result = await _geographyAgent.RunAsync(question);
        var jsonContent = result.ToString();
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        try
        {
            var geographyResponse = JsonSerializer.Deserialize<GeographyResponse>(jsonContent, jsonOptions);
            if (geographyResponse == null)
            {
                throw new InvalidOperationException($"Empty geography response received. Raw response: {jsonContent}");
            }
            return geographyResponse;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize geography response. Raw response: {jsonContent}", ex);
        }
    }
}

