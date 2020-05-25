namespace November

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open System
open Serilog
open Serilog.AspNetCore
open Serilog.Core
open Serilog.Core.Enrichers
open Serilog.Events
open Serilog.Filters
open Serilog.Formatting.Compact
open Serilog.Enrichers.AspNetCore.HttpContext

[<RequireQualifiedAccess>]
module LoggerConfiguration =
    let private loggingLevelSwitch = LoggingLevelSwitch(LogEventLevel.Information)

    let configureLogger (_: HostBuilderContext) (loggerConfiguration: LoggerConfiguration) =
        loggerConfiguration
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .WriteTo.Console(formatter = CompactJsonFormatter(), standardErrorFromLevel = Nullable<LogEventLevel>(LogEventLevel.Error))
            .MinimumLevel.ControlledBy(loggingLevelSwitch)
            |> ignore

    let updateLogLevel logLevel = loggingLevelSwitch.MinimumLevel <- logLevel

    let private getHeaders (context: HttpContext) =
        context.Request.Headers
        |> Seq.filter (fun x -> x.Value |> Seq.isEmpty |> not)
        |> Seq.map (fun x ->  x.Key, x.Value)

    let private enrichFromHeaders (diagnosticContext: IDiagnosticContext) (context: HttpContext) =
        context
        |> getHeaders
        |> Seq.iter diagnosticContext.Set

    let configureOptions (options: RequestLoggingOptions) =
        options.MessageTemplate <- "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms"
        options.EnrichDiagnosticContext <- Action<IDiagnosticContext, HttpContext>(enrichFromHeaders)

    let httpOptions =
         Action<SerilogLogContextMiddlewareOptions>(fun x ->
                x.EnrichersForContextFactory <-
                    fun context -> getHeaders context |> Seq.map (fun (k,v) -> PropertyEnricher(k,v) :> ILogEventEnricher)
             )

