using AlphaAgentWebApi.Models;

namespace AlphaAgentWebApi.Interfaces;

public interface IGeographyAgentService
{
    Task<GeographyResponse> AskGeographyAsync(string question);
}

