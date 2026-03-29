using Microsoft.AspNetCore.Mvc;
using ShopQueue.Application.Services;

namespace ShopQueue.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueuesController(IQueueService queueService) : ControllerBase
{
    [HttpGet("{queueId}")]
    public async Task<IActionResult> GetQueue(Guid queueId)
    {
        var entries = await queueService.GetEntriesAsync(queueId);
        return Ok(entries);
    }

    [HttpPost("{queueId}/call-next")]
    public async Task<IActionResult> CallNext(Guid queueId)
    {
        var entry = await queueService.CallNextAsync(queueId);
        return Ok(entry);
    }
}