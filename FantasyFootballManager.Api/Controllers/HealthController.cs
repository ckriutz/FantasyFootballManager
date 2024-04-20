using Microsoft.AspNetCore.Mvc;

namespace FantasyFootballManager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{

    public HealthController()
    {

    }

    //return okay
    [HttpGet]
    public IActionResult Get()
    {         
        return Ok("I'm alive!");
    }
}