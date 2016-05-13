module FsMyActor.OperationContracts

open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors

type IFsMyActor = 
    inherit IActor
    abstract GetCountAsync : unit -> int Task
    abstract SetCountAsync : int -> Task
    abstract RemoveCountAsync : unit -> Task
