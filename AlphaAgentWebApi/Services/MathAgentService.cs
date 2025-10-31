using AlphaAgentWebApi.Interfaces;
using AlphaAgentWebApi.Models;
using AlphaAgentWebApi.Configuration;
using AlphaAgentWebApi.Constants;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Options;

namespace AlphaAgentWebApi.Services;

public class MathAgentService : IMathAgentService
{
    private readonly AIAgent _mathAgent;

    public MathAgentService(IAgentFactory agentFactory, IOptions<AgentConfiguration> agentConfig)
    {
        var config = agentConfig.Value;
        var mathOptions = new ChatClientAgentOptions
        {
            Instructions = config.Agents[AgentNames.MathAgent].Instructions,
            Name = config.Agents[AgentNames.MathAgent].Name
        };
        
        if (config.Agents.TryGetValue(AgentNames.MathAgent, out var mathAgentSettings))
        {
            _mathAgent = agentFactory.CreateAgent(mathOptions);
        }
        else
        {
            throw new InvalidOperationException($"{AgentNames.MathAgent} not found in configuration");
        }
    }

    public async Task<AgentResponse> AskMathAsync(string question)
    {
        var result = await _mathAgent.RunAsync(question);
        return new AgentResponse
        {
            Response = result.ToString()
        };
    }
}

