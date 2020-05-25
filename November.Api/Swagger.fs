namespace November

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.OpenApi.Models
open System
open System.IO
open System.Reflection

open Swashbuckle.AspNetCore.SwaggerGen

[<RequireQualifiedAccess>]
module SwaggerConfiguration =

    let private includeXmlComments (options: SwaggerGenOptions) =
        let mutable location = Assembly.GetExecutingAssembly().Location
        let mutable xmlPath = Path.ChangeExtension(location, "xml")

        if File.Exists(xmlPath) then
            options.IncludeXmlComments(xmlPath)
        else
            location <-  (UriBuilder(Assembly.GetExecutingAssembly().CodeBase)).Path;
            xmlPath <- Path.ChangeExtension(location, "xml")

            if File.Exists(xmlPath) then
                options.IncludeXmlComments(xmlPath)


    let configureSwaggerGen (services: IServiceCollection) =
        services.AddSwaggerGen(fun options ->
            options.SwaggerDoc("v1", OpenApiInfo(Title = "", Version = "v1"))
            includeXmlComments options)

    let configureSwaggerService(appBuilder: IApplicationBuilder) =
        appBuilder
            .UseSwagger()
            .UseSwaggerUI(fun options ->
                options.SwaggerEndpoint("/swagger/v1/swagger.json", ""))
