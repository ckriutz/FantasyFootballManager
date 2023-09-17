using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using System.Text.Json;
using System.Diagnostics;


namespace FantasyFootballManager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DataStatusController : ControllerBase
{
    private readonly ILogger<DataStatusController> _logger;
    private readonly ActivitySource _activitySource;
    private readonly FantasyDbContext _context;

    public DataStatusController(ILogger<DataStatusController> logger, FantasyDbContext context, ActivitySource activitySource)
    {
        _logger = logger;
        _context = context;
        _activitySource = activitySource;
    }

    [HttpGet]
    public IEnumerable<DataStatus> GetStatuses()
    {
        using var activity = _activitySource.StartActivity("Get Statuses");
        var statuses = _context.DataStatus.ToList();
        foreach (var status in statuses)
        {
            activity?.SetTag(status.DataSource, status.LastUpdated.ToString());
        }
        return statuses;
    }
}