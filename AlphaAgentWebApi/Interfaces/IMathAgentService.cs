using AlphaAgentWebApi.Models;

namespace AlphaAgentWebApi.Interfaces;

public interface IMathAgentService
{
    Task<AgentResponse> AskMathAsync(string question);
}

