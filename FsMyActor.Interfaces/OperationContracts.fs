module FsMyActor.OperationContracts

open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors
open FsActorApp.Contracts

type IFsMyActor = 
    inherit IActor
    abstract GetCountAsync : unit -> ImmutableFsMyActorState Task
    abstract UpdateCountAsync : unit -> Task
    abstract RemoveCountAsync : unit -> Task
