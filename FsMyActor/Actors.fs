module FsActorApp.Actors

open FsMyActor.OperationContracts
open Microsoft.ServiceFabric.Actors.Runtime
open FsActorApp.ActorEventSource
open FsActorApp.Contracts

module FsMyActorActions = 
    let updateCount state = { state with Count = state.Count + 1 }

open FsMyActorActions

[<StatePersistence(StatePersistence.Persisted)>]
type internal FsMyActor() = 
    inherit Actor()
    
    override x.OnActivateAsync() = 
        ActorEventSource.Current.ActorMessage(x, "FsMyActor Activated", [||])
        x.StateManager.AddStateAsync<ImmutableFsMyActorState>("count", { Count = 0 })
    
    interface IFsMyActor with
        
        member x.UpdateCountAsync() = 
            ActorEventSource.Current.ActorMessage(x, "Updating Count", [||])
            let state = x.StateManager.GetStateAsync<ImmutableFsMyActorState>("count")
            upcast x.StateManager.AddOrUpdateStateAsync("count", state.Result, fun _ v -> v |> updateCount)
        
        member x.GetCountAsync() = 
            ActorEventSource.Current.ActorMessage(x, "Getting Count", [||])
            x.StateManager.GetStateAsync<ImmutableFsMyActorState>("count")
        
        member x.RemoveCountAsync() = 
            ActorEventSource.Current.ActorMessage(x, "Removing Count", [||])
            x.StateManager.RemoveStateAsync("count")
