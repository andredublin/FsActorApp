module FsActorApp.Program

open FsActorApp.Actors
open FsActorApp.ActorEventSource
open Microsoft.ServiceFabric.Actors
open Microsoft.ServiceFabric.Actors.Runtime
open System
open System.Threading

[<EntryPoint>]
let main argv = 
    try 
        ActorRuntime.RegisterActorAsync<FsMyActor>(fun ctx actorType -> 
            new ActorService(ctx, actorType, new Func<ActorService, ActorId, ActorBase>(fun service id -> 
                new FsMyActor(service, id) :> ActorBase))).GetAwaiter().GetResult()
        Thread.Sleep(Timeout.Infinite)
    with e -> 
        ActorEventSource.Current.ActorHostInitializationFailed(e.ToString())
        reraise()
    0
