using InMemoryQueue.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InMemoryQueue.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class JobsController : ControllerBase
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly StatusService _statusService;

    public JobsController(IBackgroundTaskQueue backgroundTaskQueue, StatusService statusService)
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _statusService = statusService;
    }

    [HttpPost]
    public async Task<IActionResult> Start()
    {
        var jobId = Guid.NewGuid();
        await _backgroundTaskQueue.QueueTaskAsync(new LongJob(jobId));
        return Ok(jobId);
    }

    [HttpGet("/status/{id:guid}")]
    public double Status(Guid id)
    {
        return _statusService.GetStatus(id);
    }
}