module FsActorApp.ActorEventSource

open System.Diagnostics.Tracing
open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors.Runtime

[<Literal>]
let MessageEventId = 1

[<Literal>]
let ActorMessageEventId = 2

[<Literal>]
let ActorHostInitializationFailedEventId = 3

[<Sealed>]
[<EventSource(Name = "MyCompany-FsActorApp-FsMyActor")>]
type internal ActorEventSource =
    inherit EventSource

    private new() = { inherit EventSource }

    static member Current : ActorEventSource = new ActorEventSource()

    static member ActorEventSource = Task.FromResult().Wait()

    [<NonEvent>]
    member x.Message (message : string, args : array<'T>) =
        if x.IsEnabled() then
            System.String.Format(message, args) |> x.Message

    [<Event(MessageEventId, Level = EventLevel.Informational, Message = "{0}")>]
    member x.Message (message : string) =
        if x.IsEnabled() then
            x.WriteEvent(MessageEventId, message)

    [<NonEvent>]
    member x.ActorMessage (actor : Actor, message, args : array<'T>) =
        if x.IsEnabled() 
           && not <| isNull actor.Id
           && not <| isNull actor.ActorService
           && not <| isNull actor.ActorService.Context
           && not <| isNull actor.ActorService.Context.CodePackageActivationContext then
            let finalMessage = System.String.Format(message, args)
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

    [<Event(ActorMessageEventId, Level = EventLevel.Informational, Message = "{9}")>]
    member private x.ActorMessage (
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

    [<Event(
        ActorHostInitializationFailedEventId, 
        Level = EventLevel.Error, 
        Message = "Actor host initialization failed", 
        Keywords = EventKeywords.None)>]
    member x.ActorHostInitializationFailed (ex : string) =
        x.WriteEvent(ActorHostInitializationFailedEventId, ex)