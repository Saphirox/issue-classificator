namespace November

type AsyncResult<'a, 'b> = Async<Result<'a, 'b>>

[<RequireQualifiedAccess>]
module AsyncResult =
    let map (mapper: 'a -> 'b) (result: AsyncResult<'a, 'c>) : AsyncResult<'b, 'c> = async {
        let! unwrappedResult = result
        return unwrappedResult |> Result.map mapper
    }

    let mapError (mapper: 'b -> 'c) (result: AsyncResult<'a, 'b>): AsyncResult<'a, 'c> = async {
        let! unwrappedResult = result
        return unwrappedResult |> Result.mapError mapper
    }

    let bind (binder: 'a -> AsyncResult<'b, 'c>) (result: AsyncResult<'a, 'c>): AsyncResult<'b, 'c> = async {
        match! result with
        | Ok value -> return! binder value
        | Error err -> return Error err
    }

    let switch f x = async {
        return f x |> Ok
    }

    let tap (f: 'a -> 'b) (x: AsyncResult<'a, 'c>): AsyncResult<'a, 'c> = async {
        let! unwrappedValue = x
        return unwrappedValue |> Result.tap f
    }

    let tapError (f: 'c -> 'b) (x: AsyncResult<'a, 'c>): AsyncResult<'a, 'c> = async {
        let! unwrappedValue = x
        return unwrappedValue |> Result.tapError f
    }

    let ignore (result: AsyncResult<_, 'a>): AsyncResult<unit, 'a> =
        map ignore result

    let tryExecute (action: Async<'a>) : AsyncResult<'a, exn> = async {
        try
            let! rs = action
            return Ok rs
        with
        | e ->
            return Error e
    }
