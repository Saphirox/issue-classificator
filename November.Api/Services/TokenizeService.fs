module November.Api.Services

open Microsoft.Extensions.Logging

type TokenizeService(logger: ILogger<TokenizeService>) =
    
    let tokenizeSymbols = [| '.' |]
    
    member __.Tokenize(text: string) =
        logger.LogInformation("Started tokenizing {Text}", text)
        
        tokenizeSymbols
        |> text.Split
        |> Array.map (fun sentence -> sentence + ".")
        |> Array.toList
        