using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace Grains;

public class ConsumerGrain : Grain, IConsumerGrain
{
    private readonly ILogger<ConsumerGrain> logger;
    private IAsyncObservable<int> consumer;
    private StreamSubscriptionHandle<int> handle;

    public ConsumerGrain(ILogger<ConsumerGrain> logger)
    {
        this.logger = logger;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        logger.LogWarning("OnActivateAsync");
        return Task.CompletedTask;
    }

    public async Task BecomeConsumer(Guid streamId, string streamNamespace, string providerToUse)
    {
        logger.LogWarning("BecomeConsumer");
        IStreamProvider streamProvider = this.GetStreamProvider(providerToUse);
        consumer = streamProvider.GetStream<int>(streamNamespace, streamId);
        handle = await consumer.SubscribeAsync(Run);
    }

    private Task Run(int item, StreamSequenceToken token)
    {
        logger.LogWarning("OnNextAsync(item={Item}, token={Token})", item, token != null ? token.ToString() : "null");
        return Task.CompletedTask;
    }

    public Task StopConsuming()
    {
        logger.LogWarning("StopConsuming");
        return handle.UnsubscribeAsync();
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        logger.LogWarning("OnDeactivateAsync");
        return Task.CompletedTask;
    }
    
}
