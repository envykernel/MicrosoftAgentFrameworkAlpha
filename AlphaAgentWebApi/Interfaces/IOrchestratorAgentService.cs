using AlphaAgentWebApi.Models;

namespace AlphaAgentWebApi.Interfaces;

public interface IOrchestratorAgentService
{
    Task<AgentResponse> AskOrchestratorAsync(string question);
}

