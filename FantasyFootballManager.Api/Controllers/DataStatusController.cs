using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using System.Text.Json;

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
        return _context.DataStatus.ToList();
    }
}