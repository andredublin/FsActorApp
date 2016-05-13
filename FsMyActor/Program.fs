module FsActorApp.Program

open System.Threading
open Microsoft.ServiceFabric.Actors.Runtime
open FsActorApp.Actors
open FsActorApp.ActorEventSource

[<EntryPoint>]
let main argv = 
    try 
        ActorRuntime.RegisterActorAsync<FsMyActor>(fun ctx actorType -> 
                new ActorService(ctx, actorType, fun () -> 
                    new FsMyActor() :> ActorBase)).GetAwaiter().GetResult()
        Thread.Sleep(Timeout.Infinite)
    with e -> 
        ActorEventSource.Current.ActorHostInitializationFailed(e.ToString())
        reraise()
    0
