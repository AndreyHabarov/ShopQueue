using MassTransit;
using Microsoft.Extensions.Logging;
using ShopQueue.Application.Messages;

namespace ShopQueue.Infrastructure.Consumers;

public class ClientCalledConsumer(ILogger<ClientCalledConsumer> logger) : IConsumer<ClientCalled>
{
    public Task Consume(ConsumeContext<ClientCalled> context)
    {
        logger.LogInformation(
            "Customer called. EntryId={EntryId}, QueueId={QueueId}, CustomerId={CustomerId}, Position={Position}",
            context.Message.QueueEntryId,
            context.Message.QueueId,
            context.Message.CustomerId,
            context.Message.Position);

        return Task.CompletedTask;
    }
}