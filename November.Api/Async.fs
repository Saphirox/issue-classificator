namespace November

open System
open System.Threading

[<AutoOpen>]
module AsyncExtensions =
    type Async with
        static member inline bind (f: 'a -> Async<'b>) (a: Async<'a>) : Async<'b> = async.Bind(a, f)

[<RequireQualifiedAccess>]
module Async =
    let switch f x = async {
        return f x
    }

    let map (mapper : 'T -> 'R) (value : Async<'T>) : Async<'R> = async {
        let! unwrappedValue = value
        let transformedValue = mapper unwrappedValue
        return transformedValue
    }

    let tap (f: 'a -> 'b) (x: Async<'a>): Async<'a> = async {
        let! unwrappedValue = x
        f unwrappedValue |> ignore

        return unwrappedValue
    }