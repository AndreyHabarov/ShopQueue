using MassTransit;
using Microsoft.Extensions.Logging;
using ShopQueue.Application.Messages;

namespace ShopQueue.Infrastructure.Consumers;

public class ClientJoinedQueueConsumer(ILogger<ClientJoinedQueueConsumer> logger) : IConsumer<ClientJoinedQueue>
{
    public Task Consume(ConsumeContext<ClientJoinedQueue> context)
    {
        logger.LogInformation(
            "Client joined queue. EntryId={EntryId}, QueueId={QueueId}, CustomerId={CustomerId}, Position={Position}",
            context.Message.QueueEntryId,
            context.Message.QueueId,
            context.Message.CustomerId,
            context.Message.Position);

        return Task.CompletedTask;
    }
}