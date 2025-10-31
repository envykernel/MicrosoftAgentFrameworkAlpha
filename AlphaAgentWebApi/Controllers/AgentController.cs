using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AlphaAgentWebApi.Interfaces;
using AlphaAgentWebApi.Models;
using AlphaAgentWebApi.Configuration;
using Microsoft.Agents.AI;
using System.Text.Json;

namespace AlphaAgentWebApi.Controllers;
 
[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly AIAgent _geographyAgent;
    private readonly AIAgent _mathAgent;

    public AgentController(IAgentFactory agentFactory, IOptions<AgentConfiguration> agentConfig)
    {
        var config = agentConfig.Value;
        var geographyOptions = new ChatClientAgentOptions
        {
            Instructions = config.Agents["GeographyAgent"].Instructions,
            Name = config.Agents["GeographyAgent"].Name,
            ChatOptions = new()
            {
                ResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat.ForJsonSchema<GeographyResponse>()
            }
        };
        
        // Create Geography Agent
        if (config.Agents.TryGetValue("GeographyAgent", out var geoAgentSettings))
        {
            _geographyAgent = agentFactory.CreateAgent(geographyOptions);
        }
        else
        {
            throw new InvalidOperationException("GeographyAgent not found in configuration");
        }

        // Create Math Agent
        var mathOptions = new ChatClientAgentOptions
        {
            Instructions = config.Agents["MathAgent"].Instructions,
            Name = config.Agents["MathAgent"].Name
        };
        if (config.Agents.TryGetValue("MathAgent", out var mathAgentSettings))
        {
            _mathAgent = agentFactory.CreateAgent(mathOptions);
        }
        else
        {
            throw new InvalidOperationException("MathAgent not found in configuration");
        }
    }

    [HttpPost("geography")]
    public async Task<ActionResult<GeographyResponse>> AskGeography([FromBody] AgentRequest request)
    {
        var result = await _geographyAgent.RunAsync(request.Question);
        var jsonContent = result.ToString();
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        try
        {
            var geographyResponse = JsonSerializer.Deserialize<GeographyResponse>(jsonContent, jsonOptions);
            return geographyResponse != null 
                ? Ok(geographyResponse) 
                : BadRequest(new { error = "Empty geography response received", rawResponse = jsonContent });
        }
        catch (JsonException ex)
        {
            return BadRequest(new { error = "Failed to deserialize geography response", rawResponse = jsonContent, exception = ex.Message });
        }
    }

    [HttpPost("math")]
    public async Task<ActionResult<AgentResponse>> AskMath([FromBody] AgentRequest request)
    {
        var result = await _mathAgent.RunAsync(request.Question);
        var response = new AgentResponse
        {
            Response = result.ToString()
        };

        return Ok(response);
    }
}
