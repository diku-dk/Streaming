# Streaming

Streaming is a simple Orleans-based application to understand how to setup Orleans Streams.

## Table of Contents
- [Getting Started](#getting-started)
    * [Prerequisites](#prerequisites)
    * [New Orleans Users](#orleans)
    * [How to Run](#run)
- [Exercise](#exercise)
    * [Description](#description)
    * [Solution Direction](#solution)

## <a name="getting-started"></a>Getting Started

### <a name="prerequisites"></a>Prerequisites

- IDE: [Visual Studio](https://visualstudio.microsoft.com/vs/community/) or [VSCode] (https://code.visualstudio.com/)
- [.NET Framework 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

### <a name="orleans"></a>New Orleans Users

[Orleans](https://learn.microsoft.com/en-us/dotnet/orleans/) framework provide facilities to program distributed applications at scale using the virtual actor model. We highly recommend starting from the [Orleans Documentation](https://learn.microsoft.com/en-us/dotnet/orleans/overview) to further understand the model.

### <a name="run"></a>How to Run

In the project's root folder, run the following command:

```
dotnet run --project Silo
```

This command will start up the Orleans server (aka silo) and submit a message to an actor. The program finishes upon any key pressed by the user.

To open the project in Visual Studio, make sure to select the Streaming.sln as the solution file, so Visual Studio will recognize the solution as a whole and allow you to debug your application.

## <a name="exercise"></a>Exercise

### <a name="description"></a>Description

Starting from the project sample provided, implement the following scenario:

1. Two actors exchange events up to a point one of them decides to terminate the communication 
2. When the termination is decided by one of them, a special event must be sent to the other actor
3. After the other actor receives the termination, no events across actors can be exchanged anymore
4. Besides, the application must terminate next

Whether the next event to be generated is a termination can be decided randomly. For example, pick a number from 1 to 10, if it is 0, then it should terminate.
The started event may be sent from the main method (like what you have now in the code).
Promise primitives may come in handy. In other words, the main thread need to wait asynchronously for the termination.

### <a name="solution"></a>Solution Direction

First, we need different streams in order to setup the communication channel between the actors.
Particularly, we need one stream per actor.


Considering the channel found in Silo/Program.cs to start the scenario is C1 (main -> actor 1), we need two other streams: C2 (actor 1 -> actor 2) and C3 (actor 2 -> actor 1).


Each stream is uniquely identified by two attributes, a Guid and a namespace:
```
Guid "AD713788-B5AE-49FF-8B2C-F311B9CB0CC1"
Namespace "RANDOMDATA"
```

For this exercise, you can assume the namespace does not change, so we only need to switch the Guid for each stream. For that, just change the last number of Guid sequence (e.g., switch 1 to 2).


Once we define our streams, now we need to make sure the actors subscribe to their correct streams. The following method may be useful:
```
GetPrimaryKeyLong()
```

That will provide the ID of the actor and then you can use that information to subscribe to its corresponding stream.

Once actor 1 subscribes to the correct stream, upon receiving a message from the actor 2, it must decide whether the next message to send to actor 2 is a termination. A simple way to accomplish that is found below:

```
var random = new Random();
// ...
int val = random.NextInt64(1,10);
if(val == 1)
    // send termination event
    // unsubscribe from C3 and don't send any messages anymore to C2
```

Make sure that you publish the event to the correct stream (not the same stream you have received the event!).

Once an actor receives a termination, we need to find a way to inform the main thread about the termination, so the program can finish. Two of the possible options are:

1. Polling: Create a "Finished" method in the consumer actor class and, in the main, periodically asks both actors whether they are both finished
2. Create a special channel for acknowledging termination (C4): Send a message to C4 and make sure that main thread subscribe to C4 before publishing the starter event (in C1). In this case, you have to make sure to synchronize the main thread with the thread triggered by the subscription.

```
public Task<bool> Finished()
{
    // ...
}
```

