using LogService.Application.Commands.CreateLog;
using LogService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogService.API.Controllers;

[ApiController]
[Route("api/logs")]
public class LogController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogRepository _logRepository;

    public LogController(ISender sender, ILogRepository logRepository)
    {
        _sender = sender;
        _logRepository = logRepository;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLogCommand command)
    {
        await _sender.Send(command);
        return StatusCode(StatusCodes.Status201Created);
    }

    [AllowAnonymous]
    [HttpGet("{service}")]
    public async Task<IActionResult> GetByService(string service)
    {
        var logs = await _logRepository.GetByServiceAsync(service);
        return Ok(logs);
    }

    [AllowAnonymous]
    [HttpGet("level/{level}")]
    public async Task<IActionResult> GetByLevel(string level)
    {
        var logs = await _logRepository.GetByLevelAsync(level);
        return Ok(logs);
    }
}
