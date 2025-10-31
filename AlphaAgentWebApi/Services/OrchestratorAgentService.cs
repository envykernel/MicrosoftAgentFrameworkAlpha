using AlphaAgentWebApi.Interfaces;
using AlphaAgentWebApi.Models;
using Microsoft.Agents.AI;

namespace AlphaAgentWebApi.Services;

public class OrchestratorAgentService : IOrchestratorAgentService
{
    private readonly AIAgent _orchestratorAgent;
    private readonly AIAgent _geographyAgent;
    private readonly AIAgent _mathAgent;

    public OrchestratorAgentService(IAgentProvider agentProvider)
    {
        _orchestratorAgent = agentProvider.GetOrchestratorAgent();
        _geographyAgent = agentProvider.GetGeographyAgent();
        _mathAgent = agentProvider.GetMathAgent();
    }

    public async Task<AgentResponse> AskOrchestratorAsync(string question, AgentThread? thread = null)
    {
        if (thread == null)
        {
            thread = _orchestratorAgent.GetNewThread(); 
        }

        var result = await _orchestratorAgent.RunAsync(question, thread);
        return new AgentResponse
        {
            Response = result.ToString()
        };
    }
}

