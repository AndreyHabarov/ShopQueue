using MassTransit;
using ShopQueue.Application.Messages;

namespace ShopQueue.Infrastructure.Consumers;

public class ClientCalledConsumer : IConsumer<ClientCalled>
{
    public Task Consume(ConsumeContext<ClientCalled> context)
    {
        var msg = context.Message;
        Console.WriteLine($"[ClientCalled] Entry={msg.QueueEntryId}, Position={msg.Position}");
        return Task.CompletedTask;
    }
}