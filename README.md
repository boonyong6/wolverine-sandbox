# Getting Started

- `IMessageBus` 
  - One of the **core types** in Wolverine.
  - Scoped service.
  - Registered by `UseWolverine()`.
- \*Use a [naming **convention**](https://wolverinefx.net/guide/handlers/#rules-for-message-handlers) to discover message handler actions.
- Can use **static methods** as handler actions.

# \*Message Handlers

- Whole purpose of Wolverine is **to connect incoming messages to handling code (handler)**.
- Things happen between `IMessageBus.PublishAsync()` and `MyMessageHandler.Handle(MyMessage)`:
  1. Discover handlers (correlate by message type)
  2. Build the connection between message and handler at runtime.
- A message can have **many handlers**. Each will be called in sequence they were discovered.

## Rules for Message Handlers

- **Message handlers** must be **public types** with a **public constructor**.
- **Handler methods** must also be **public**.
- **Message type** must be **public**.
- **First argument** of the handler method must be the **message type**.
- Interface or abstract class as a message type is acceptable, BUT!!!
- Naming conventions:
  - **Handler type** should be suffixed with either `Handler` or `Consumer`.
  - **Handler method** should be either `Handle()` or `Consume()`.

## Multiple Handlers for the Same Message Type

- **_Original_ concept** - Combine multiple handlers into one logical handler that **executes together**, and in the **same logical transaction** (if you use **transactional middleware**).
  - **NOT SUITABLE** for **"Modular Monolith"** or **"Event Driven Architecture"** where you **take independent actions on the same message**.
- **_Sticky Handler_ concept** - "stick" them against **different listening endpoints** or **local queues**.
  - **WARNING** - `Separated` setting is ignored by `Saga` handlers.
  
  ```csharp
	using var host = Host.CreateDefaultBuilder()
		.UseWolverine(opts =>
		{
			// Right here, tell Wolverine to make every handler "sticky"
			opts.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;
		}).StartAsync();
  ```
- Create a **separate listener** for each handler using the handler type.

## \*Message Handler Parameters

- First argument - Message type
- After that, accept:
  - Services from IoC
  - `Envelope` - Contains **message metadata**
  - `IMessageContext` or `IMessageBus`
  - `CancellationToken`
  - `DateTime now` or `DateTimeOffset now`

## Handler Lifecycle & Service Dependencies

- **TIP:** Using a **static method** as your message handler can be a small performance improvement by avoiding the need to create and garbage collect new objects at runtime.

## "Compound Handlers"

- Split message handling for a **single message** up into methods that **load data** (data access) and **business logic** or decides to take other actions (branching).
  - **Note:** Also improve **testability**.
- Can use Wolverine's [conventional middleware naming conventions](https://wolverinefx.net/guide/handlers/middleware.html#conventional-middleware).
- **The Wolverine's way** of handling separation of concerns.
- **Naming conventions** that determine methods to be either **"before"** or **"after"**:
  
  Lifecycle | Method Names
  -|-
  Before the Handler(s) | `Before`, `BeforeAsync`, `Load`, `LoadAsync`, `Validate`, `ValidateAsync`
  After the Handler(s) | `After`, `AfterAsync`, `PostProcess`, `PostProcessAsync`
  In `finally` blocks after the Handlers & "After" methods | `Finally`, `FinallyAsync`

- **Alternative** - Can use `[Before]` or `[After]` attributes with more descriptive method names.
- \*Wolverine will **reorder** the methods when one method produces an input to another method.

# Messaging

## Introduction to Messaging - Getting Started with Wolverine as Message Bus

- Info: Can connect to multiple message broker instances from one app.

### Configuring Messaging

- Components to configure:
  1. **Connectivity** (to external transports)
       - Configure via an extension method on `WolverineOptions` using the `Use[ToolName]()`. 
  2. **Listening endpoints**
  3. **Routing rules** - Where and how to send/publish messages.
- Supported transports:
  - [TCP transport](https://wolverinefx.net/guide/messaging/transports/tcp.html)
  - ["local" in memory queues](https://wolverinefx.net/guide/messaging/transports/local.html)
  - **External infrastructure** - RabbitMQ, Azure Service Bus, Amazon SQS, Amazon SNS, Google PubSub, Apache Pulsar, Sql Server, PostgreSQL, MQTT, Kafka, External Database Tables

## Sending Messages with IMessageBus

- Main entry point - `IMessageBus`
- Second abstraction - `IMessageContext`
  - Can be optionally consumed within message handlers to add some extra operations and metadata.
- Sample usage of the **most common operations**:

  ```csharp
  public static async Task use_message_bus(IMessageBus bus)
  {
      // Execute this command message right now! And wait until
      // it's completed or acknowledged
      await bus.InvokeAsync(new DebitAccount(1111, 100));

      // Execute this message right now, but wait for the declared response
      var status = await bus.InvokeAsync<AccountStatus>(new DebitAccount(1111, 250));

      // Send the message expecting there to be at least one subscriber to be executed later, but
      // don't wait around
      await bus.SendAsync(new DebitAccount(1111, 250));

      // Or instead, publish it to any interested subscribers,
      // but don't worry about it if there are actually any subscribers
      // This is probably best for raising event messages
      await bus.PublishAsync(new DebitAccount(1111, 300));

      // Send a message to be sent or executed at a specific time
      await bus.ScheduleAsync(new DebitAccount(1111, 100), DateTimeOffset.UtcNow.AddDays(1));

      // Or do the same, but this time express the time as a delay
      await bus.ScheduleAsync(new DebitAccount(1111, 225), 1.Days());
  }
  ```
