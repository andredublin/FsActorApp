module FsActorApp.ActorEventSource

open System
open System.Diagnostics.Tracing
open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors.Runtime

[<Literal>]
let MessageEventId = 1

[<Literal>]
let ActorMessageEventId = 2

[<Literal>]
let ActorHostInitializationFailedEventId = 3

[<Literal>]
let HostInitialization : EventKeywords = LanguagePrimitives.EnumOfValue 0x1L

[<Sealed>]
[<EventSource(Name = "MyCompany-FsActorApp-FsMyActor")>]
type internal ActorEventSource =
    inherit EventSource

    // Instance constructor is private to enforce singleton semantics
    private new() = { inherit EventSource }

    static member Current = new ActorEventSource()

    // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
    // This problem will be fixed in .NET Framework 4.6.2.
    static member ActorEventSource = Task.FromResult().Wait()    

    // Define an instance method for each event you want to record and apply an [Event] attribute to it.
    // The method name is the name of the event.
    // Pass any parameters you want to record with the event (only primitive integer types, DateTime, Guid & string are allowed).
    // Each event method implementation should check whether the event source is enabled, and if it is, call WriteEvent() method to raise the event.
    // The number and types of arguments passed to every event method must exactly match what is passed to WriteEvent().
    // Put [NonEvent] attribute on all methods that do not define an event.
    // For more information see https://msdn.microsoft.com/en-us/library/system.diagnostics.tracing.eventsource.aspx
    [<NonEvent>]
    member x.Message (message, args) =
        if x.IsEnabled() then
            String.Format(message, args) |> x.Message

    [<Event(MessageEventId, Level = EventLevel.Informational, Message = "{0}")>]
    member x.Message (message) =
        if x.IsEnabled() then
            x.WriteEvent(MessageEventId, message)

    [<NonEvent>]
    member x.ActorMessage (actor : Actor, message, args) =
        if x.IsEnabled() 
           && not <| isNull actor.Id
           && not <| isNull actor.ActorService
           && not <| isNull actor.ActorService.Context
           && not <| isNull actor.ActorService.Context.CodePackageActivationContext then
            let finalMessage = String.Format(message, args)
            x.ActorMessage(
                actor.GetType().ToString(),
                actor.Id.ToString(),
                actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                actor.ActorService.Context.ServiceTypeName,
                actor.ActorService.Context.ServiceName.ToString(),
                actor.ActorService.Context.PartitionId,
                actor.ActorService.Context.ReplicaId,
                actor.ActorService.Context.NodeContext.NodeName,
                finalMessage)

    // For very high-frequency events it might be advantageous to raise events using WriteEventCore API.
    // This results in more efficient parameter handling, but requires explicit allocation of EventData structure and unsafe code.
    // To enable this code path, define UNSAFE conditional compilation symbol and turn on unsafe code support in project properties.
    [<Event(ActorMessageEventId, Level = EventLevel.Informational, Message = "{9}")>]
    member private 
#if UNSAFE
            unsafe
#endif
        x.ActorMessage (
                        actorType, 
                        actorId, 
                        applicationTypeName, 
                        applicationName, 
                        serviceTypeName, 
                        serviceName, 
                        partitionId, 
                        replicaOrInstanceId, 
                        nodeName, 
                        message) =
#if !UNSAFE
        x.WriteEvent(
            ActorMessageEventId, 
            actorType,
            actorId,
            applicationTypeName,
            applicationName,
            serviceTypeName,
            serviceName,
            partitionId,
            replicaOrInstanceId,
            nodeName,
            message)
#else
    // waiting on F# 4.1 to use the fixed keyword
#endif

    [<Event(
        ActorHostInitializationFailedEventId, 
        Level = EventLevel.Error, 
        Message = "Actor host initialization failed", 
        Keywords = HostInitialization)>]
    member x.ActorHostInitializationFailed (ex : string) =
        x.WriteEvent(ActorHostInitializationFailedEventId, ex)
    
#if UNSAFE
    member private x.SizeInBytes (s : string) =
        if isNull <| s then 0
        else s.Length + 1 * sizeof<char>
#endif