using Microsoft.AspNetCore.Mvc;
using ShopQueue.Application.Services;
using ShopQueue.Responses;

namespace ShopQueue.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueuesController(IQueueService queueService) : ControllerBase
{
    [HttpGet("{queueId}")]
    public async Task<IActionResult> GetQueue(Guid queueId, CancellationToken cancellationToken)
    {
        var entries = await queueService.GetEntriesAsync(queueId, cancellationToken);
        return Ok(entries.Select(x => QueueEntryResponse.FromEntity(x)));
    }

    [HttpPost("{queueId}/call-next")]
    public async Task<IActionResult> CallNext(Guid queueId, CancellationToken cancellationToken)
    {
        var entry = await queueService.CallNextAsync(queueId, cancellationToken);
        return Ok(QueueEntryResponse.FromEntity(entry));
    }

    [HttpPatch("{queueId}/entries/{entryId}/served")]
    public async Task<IActionResult> MarkAsServed(Guid queueId, Guid entryId, CancellationToken cancellationToken)
    {
        var entry = await queueService.MarkAsServedAsync(queueId, entryId, cancellationToken);
        return Ok(QueueEntryResponse.FromEntity(entry));
    }

    [HttpDelete("{queueId}/entries/{entryId}")]
    public async Task<IActionResult> CancelEntry(Guid queueId, Guid entryId, CancellationToken cancellationToken)
    {
        await queueService.CancelEntryAsync(queueId, entryId, cancellationToken);
        return NoContent();
    }
}