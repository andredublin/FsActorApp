namespace FsActorApp.Contracts

[<AutoOpen>]
module DataContracts =

    open System.Runtime.Serialization

    [<DataContract>]
    [<CLIMutable>]
    type ImmutableFsMyActorState = 
        { [<DataMember>]
            Count : int }