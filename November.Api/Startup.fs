namespace November

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open November.Api.Services
open November.Config
open Serilog.Enrichers.AspNetCore.HttpContext
open Serilog
open System.Text.Json

type Startup(configuration: IConfiguration) =
    
    member this.ConfigureServices(services: IServiceCollection) =
        services.AddHttpClient() |> ignore
        services.Configure<IssueApiConfig>(configuration.GetSection("IssueApi")) |> ignore 
        services
            .AddTransient<IssueService>()
            .AddTransient<TokenizeService>() |> ignore
        services.AddLogging() |> ignore
        
        services.AddControllers()
                .AddJsonOptions(fun options -> options.JsonSerializerOptions.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase) |> ignore
        services.AddCors() |> ignore
        SwaggerConfiguration.configureSwaggerGen(services) |> ignore
        
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseSerilogRequestLogging(LoggerConfiguration.configureOptions) |> ignore
        app.UseSerilogLogContext(LoggerConfiguration.httpOptions) |> ignore
                
        SwaggerConfiguration.configureSwaggerService(app) |> ignore

        app.UseRouting() |> ignore
        app.UseEndpoints(fun endpoints -> endpoints.MapControllers() |> ignore) |> ignore
