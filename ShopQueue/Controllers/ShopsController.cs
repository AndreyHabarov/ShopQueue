using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Enums;
using ShopQueue.Requests;
using ShopQueue.Responses;

namespace ShopQueue.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRole.Owner)]
public class ShopsController(IShopService shopService, IQueueService queueService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateShop([FromBody] CreateShopRequest request,
        CancellationToken cancellationToken)
    {
        var shop = await shopService.CreateAsync(request.Name, request.Address, cancellationToken);
        return CreatedAtAction(nameof(CreateShop), new { id = shop.Id }, ShopResponse.FromEntity(shop));
    }

    [HttpPost("{shopId}/queues")]
    public async Task<IActionResult> CreateQueue(Guid shopId, [FromBody] CreateQueueRequest request,
        CancellationToken cancellationToken)
    {
        var queue = await queueService.CreateAsync(shopId, request.Name, cancellationToken);
        return CreatedAtAction(nameof(CreateQueue), new { shopId, id = queue.Id }, QueueResponse.FromEntity(queue));
    }

    [HttpGet("{shopId}")]
    public async Task<IActionResult> GetShop(Guid shopId, CancellationToken cancellationToken)
    {
        var shop = await shopService.GetByIdAsync(shopId, cancellationToken);
        if (shop is null)
            return NotFound();

        return Ok(ShopResponse.FromEntity(shop));
    }

    [HttpGet("{shopId}/queues")]
    public async Task<IActionResult> GetQueues(Guid shopId, CancellationToken cancellationToken)
    {
        var queues = await queueService.GetByShopIdAsync(shopId, cancellationToken);
        return Ok(queues.Select(QueueResponse.FromEntity));
    }
}