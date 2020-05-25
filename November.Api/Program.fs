namespace November

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Serilog

module Program =
    let exitCode = 0

    let CreateHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder.UseStartup<Startup>() |> ignore
            )
            .UseSerilog(LoggerConfiguration.configureLogger)

    [<EntryPoint>]
    let main args =
        CreateHostBuilder(args).Build().Run()
        exitCode
