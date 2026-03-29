using Microsoft.AspNetCore.Mvc;
using ShopQueue.Application.Services;
using ShopQueue.Requests;

namespace ShopQueue.Controllers;

[ApiController]
[Route("api/queues")]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    [HttpPost("{queueId}/join")]
    public async Task<IActionResult> JoinQueue(Guid queueId, [FromBody] JoinQueueRequest request)
    {
        var entry = await customerService.JoinQueueAsync(queueId, request.CustomerName, request.CustomerPhone);
        return Ok(entry);
    }
}