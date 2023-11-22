using System;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface IConsumerGrain : IGrainWithIntegerKey
    {
        Task BecomeConsumer(Guid streamId, string streamNamespace, string providerToUse);

        Task StopConsuming();
    }
}
