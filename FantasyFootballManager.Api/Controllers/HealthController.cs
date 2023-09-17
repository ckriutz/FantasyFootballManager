using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace FantasyFootballManager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    public static ActivitySource activitySource = new ActivitySource("FantasyFootballManager.Api");
    private readonly ActivitySource _activitySource;

    public HealthController(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    //return okay
    [HttpGet]
    public IActionResult Get()
    {         
        using (Activity activity = _activitySource.StartActivity("HealthController.Get"))
        {
            activity?.SetTag("message", "oh shit this works.");
        }
        return Ok("I'm alive!");
        
    }
}