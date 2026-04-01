using Microsoft.AspNetCore.Mvc;
using ShopQueue.Application.Services;
using ShopQueue.Requests;
using ShopQueue.Responses;

namespace ShopQueue.Controllers;

[ApiController]
[Route("api/queues")]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    [HttpPost("{queueId}/join")]
    public async Task<IActionResult> JoinQueue(Guid queueId, [FromBody] JoinQueueRequest request,
        CancellationToken cancellationToken)
    {
        var entry = await customerService.JoinQueueAsync(queueId, request.CustomerName, request.CustomerPhone, cancellationToken);
        return CreatedAtAction(nameof(JoinQueue), new {queueId, id = entry.Id},QueueEntryResponse.FromEntity(entry, request.CustomerName));
    }
}