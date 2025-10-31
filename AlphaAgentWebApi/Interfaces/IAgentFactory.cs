using Microsoft.Agents.AI;

namespace AlphaAgentWebApi.Interfaces;

public interface IAgentFactory
{
    AIAgent CreateAgent(ChatClientAgentOptions options);
    AIAgent CreateAgent(ChatClientAgentOptions options, string? deploymentName, string? endpoint);

}
