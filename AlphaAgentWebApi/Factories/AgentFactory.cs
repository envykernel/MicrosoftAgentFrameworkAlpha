using AlphaAgentWebApi.Interfaces;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using AlphaAgentWebApi.Configuration;

namespace AlphaAgentWebApi.Factories;

public class AgentFactory : IAgentFactory
{
    private readonly string _deploymentName;
    private readonly string _endpoint;
    private readonly DefaultAzureCredential _credential;
    private readonly OpenAI.Chat.ChatClient _chatClient;

    public AgentFactory(IOptions<AgentConfiguration> configOptions)
    {
        var cfg = configOptions.Value;

        _deploymentName =
            Environment.GetEnvironmentVariable(cfg.PsDeploymentEnvName)
            ?? throw new ArgumentNullException(nameof(cfg.PsDeploymentEnvName), $"{cfg.PsDeploymentEnvName} environment variable is not set");

        _endpoint =
            Environment.GetEnvironmentVariable(cfg.PsEndpointEnvName)
            ?? throw new ArgumentNullException(nameof(cfg.PsEndpointEnvName), $"{cfg.PsEndpointEnvName} environment variable is not set");

        var authOptions = new DefaultAzureCredentialOptions { ExcludeAzureDeveloperCliCredential = false };
        _credential = new DefaultAzureCredential(authOptions);

        _chatClient = new AzureOpenAIClient(new Uri(_endpoint), _credential)
            .GetChatClient(_deploymentName);
    }

    public AIAgent CreateAgent(ChatClientAgentOptions options)
    {
        return _chatClient.CreateAIAgent(options);
    }

    public AIAgent CreateAgent(ChatClientAgentOptions options, string? deploymentName, string? endpoint)
    {
        bool hasDeployment = !string.IsNullOrWhiteSpace(deploymentName);
        bool hasEndpoint = !string.IsNullOrWhiteSpace(endpoint);

        if (hasDeployment ^ hasEndpoint)
        {
            throw new ArgumentException("Both deploymentName and endpoint must be provided together when overriding.");
        }

        string effectiveDeployment = hasDeployment ? deploymentName! : _deploymentName;
        string effectiveEndpoint = hasEndpoint ? endpoint! : _endpoint;

        ArgumentException.ThrowIfNullOrWhiteSpace(effectiveDeployment);
        ArgumentException.ThrowIfNullOrWhiteSpace(effectiveEndpoint);

        return new AzureOpenAIClient(new Uri(effectiveEndpoint), _credential)
            .GetChatClient(effectiveDeployment)
            .CreateAIAgent(options);
    }

}



