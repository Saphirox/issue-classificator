namespace November

[<RequireQualifiedAccess>]
module Result =
    let tap (f: 'a -> 'b) (x: Result<'a, 'c>): Result<'a, 'c> =
        match x with
        | Ok value -> 
            f value |> ignore
            x
        | _ -> x

    let tapError (f: 'c -> 'b) (x: Result<'a, 'c>): Result<'a, 'c> =
        match x with
        | Error err -> 
            f err |> ignore
            x
        | _ -> x

    let getOrThrow (r: Result<_, _>) =
        match r with
        | Ok ok -> ok
        | Error error -> error |> raise

    let bindAsync (binder : 'a -> Async<Result<'b, 'c>>) (result : Result<'a, 'c>) : Async<Result<'b, 'c>> =
        match result with
        | Ok value -> binder value
        | Error err -> async.Return (Error err)

    let isOk =
        function
        | Ok _ -> true
        | Error _ -> false

    let isError x =
        x |> isOk |> not

    let tryExecute f x =
        try
            Ok (f x)
        with
        | e ->
            Error e

    let getError =
        function
        | Error error -> error
        | Ok _ -> failwith "Result value expected to be Error, but there is Ok"

    let combineErrors<'error> : Result<unit, 'error> seq -> Result<unit, 'error list> =
        let combiner state current =
            match current, state with
            | Ok _, Error x -> Error x
            | Error a, Error b -> b |> List.append [ a ] |> Error
            | Ok _, Ok _ -> Ok ()
            | Error a, Ok _ -> [ a ] |> Error

        let init = Ok ()
        List.ofSeq >> List.fold combiner init
