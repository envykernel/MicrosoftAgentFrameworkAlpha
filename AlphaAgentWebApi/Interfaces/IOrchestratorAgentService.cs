using AlphaAgentWebApi.Models;
using Microsoft.Agents.AI;

namespace AlphaAgentWebApi.Interfaces;

public interface IOrchestratorAgentService
{
    Task<AgentResponse> AskOrchestratorAsync(string question, AgentThread? thread = null);
}

