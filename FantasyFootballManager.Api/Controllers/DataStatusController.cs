using Microsoft.AspNetCore.Mvc;

namespace FantasyFootballManager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DataStatusController : ControllerBase
{
    private readonly ILogger<DataStatusController> _logger;
    private readonly FantasyDbContext _context;

    public DataStatusController(ILogger<DataStatusController> logger, FantasyDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IEnumerable<DataStatus> GetStatuses()
    {
        var statuses = _context.DataStatus.ToList();
        return statuses;
    }
}