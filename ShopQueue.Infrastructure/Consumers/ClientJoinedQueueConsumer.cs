using MassTransit;
using ShopQueue.Application.Messages;

namespace ShopQueue.Infrastructure.Consumers;

public class ClientJoinedQueueConsumer : IConsumer<ClientJoinedQueue>
{
    public Task Consume(ConsumeContext<ClientJoinedQueue> context)
    {
        var msg = context.Message;
        Console.WriteLine($"[ClientJoinedQueue] Entry={msg.QueueEntryId}, Position={msg.Position}");
        return Task.CompletedTask;
    }
}