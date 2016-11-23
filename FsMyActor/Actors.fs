module FsActorApp.Actors

open FsMyActor.OperationContracts
open Microsoft.ServiceFabric.Actors.Runtime
open FsActorApp.ActorEventSource
open FsActorApp.Contracts

module FsMyActorActions = 
    let updateCount state = { state with Count = state.Count + 1 }

open FsMyActorActions

/// <remarks>
/// This class represents an actor.
/// Every ActorID maps to an instance of this class.
/// The StatePersistence attribute determines persistence and replication of actor state:
///  - Persisted: State is written to disk and replicated.
///  - Volatile: State is kept in memory only and replicated.
///  - None: State is kept in memory only and not replicated.
/// </remarks>
[<StatePersistence(StatePersistence.Persisted)>]
type internal FsMyActor(actorService, actorId) = 
    inherit Actor(actorService, actorId)
    
    /// <summary>
    /// This method is called whenever an actor is activated.
    /// An actor is activated the first time any of its methods are invoked.
    /// </summary>
    override x.OnActivateAsync() = 
        ActorEventSource.Current.ActorMessage(x, "FsMyActor Activated", [||])
        // The StateManager is this actor's private state store.
        // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
        // Any serializable object can be saved in the StateManager.
        // For more information, see http://aka.ms/servicefabricactorsstateserialization
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
