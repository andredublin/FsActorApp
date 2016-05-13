module FsActorApp.Actors

open FsMyActor.OperationContracts
open Microsoft.ServiceFabric.Actors.Runtime
open FsActorApp.ActorEventSource

[<StatePersistence(StatePersistence.Persisted)>]
type internal FsMyActor() = 
    inherit Actor()

    override x.OnActivateAsync() =
        ActorEventSource.Current.ActorMessage(x, "ActorActivated", [||])
        x.StateManager.AddStateAsync("count", 0)

    interface IFsMyActor with
        
        member x.SetCountAsync count = 
            ActorEventSource.Current.ActorMessage(x, "Updating Count", [||])
            upcast x.StateManager.AddOrUpdateStateAsync("count", count, fun k v -> if count > v then count else v)
        
        member x.GetCountAsync() = 
            ActorEventSource.Current.ActorMessage(x, "Getting Count", [||])
            x.StateManager.GetStateAsync<int>("count")

        member x.RemoveCountAsync() =
            ActorEventSource.Current.ActorMessage(x, "Removing Count", [||])
            x.StateManager.RemoveStateAsync("count")