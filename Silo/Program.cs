using Orleans;
using Orleans.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System;
using Microsoft.Extensions.DependencyInjection;
using GrainInterfaces;

using var host = new HostBuilder()
    .UseOrleans(builder =>
    {
        builder
            .UseLocalhostClustering()
            .AddMemoryStreams("StreamProvider")
            .AddMemoryGrainStorage("PubSubStore")
            .ConfigureLogging(logging =>
             {
                 logging.ClearProviders();
                 logging.AddConsole();
                 logging.SetMinimumLevel(LogLevel.Warning);
             })
            ;
    })
    .Build();


await host.StartAsync();

// Send events to your grains via Orleans Streams

var client = host.Services.GetService<IClusterClient>();
var guid = new Guid("AD713788-B5AE-49FF-8B2C-F311B9CB0CC1");

// 1 - Initialize the grain first
var grain = client.GetGrain<IConsumerGrain>(0);

await grain.BecomeConsumer(guid, "RANDOMDATA", "StreamProvider");

// 2 - Get one of the providers which we defined in our config
var streamProvider = client.GetStreamProvider("StreamProvider");

// 3 - Get the reference to a stream
var streamId = StreamId.Create("RANDOMDATA", guid);
var stream = streamProvider.GetStream<int>(streamId);

// 4 - Send event in the stream
await stream.OnNextAsync(1);

Console.WriteLine("            The Orleans server started. Press any key to terminate...         ");
Console.WriteLine("\n *************************************************************************");

Console.ReadLine();

await grain.StopConsuming();

await host.StopAsync();