using Microsoft.AspNetCore.Mvc;
using AlphaAgentWebApi.Interfaces;
using AlphaAgentWebApi.Models;

namespace AlphaAgentWebApi.Controllers;
 
[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly IGeographyAgentService _geographyAgentService;
    private readonly IMathAgentService _mathAgentService;

    public AgentController(IGeographyAgentService geographyAgentService, IMathAgentService mathAgentService)
    {
        _geographyAgentService = geographyAgentService;
        _mathAgentService = mathAgentService;
    }

    [HttpPost("geography")]
    public async Task<ActionResult<GeographyResponse>> AskGeography([FromBody] AgentRequest request)
    {
        try
        {
            var geographyResponse = await _geographyAgentService.AskGeographyAsync(request.Question);
            return Ok(geographyResponse);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("math")]
    public async Task<ActionResult<AgentResponse>> AskMath([FromBody] AgentRequest request)
    {
        var response = await _mathAgentService.AskMathAsync(request.Question);
        return Ok(response);
    }
}
