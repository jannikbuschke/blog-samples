#nowarn "20"

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Http

open SharedModels
open Fable.Remoting.Server
open Fable.Remoting.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting


open System.Threading
open System.Threading.Tasks
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Http
open System
open System.Reflection
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging


module Program =
    let exitCode = 0

    let webApp: RemotingOptions<HttpContext, MovieService> =
        Remoting.createApi ()
        |> Remoting.fromValue Server.HelloWorldService.service

    [<EntryPoint>]
    let main args =

        let builder =
            WebApplication.CreateBuilder(args)

        builder.Services.AddControllers()
        builder.Services.AddSpaStaticFiles(fun o ->
            o.RootPath <- "www"
            )

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseRouting()
        app.UseAuthorization()

        // Add the API to the ASP.NET Core pipeline
        app.UseRemoting(webApp)
        // you can have multiple API's
        // app.UseRemoting(otherApp)

        app.MapControllers()

        app.UseStaticFiles()

        app.UseSpaStaticFiles()
        
        app.UseSpa (fun spa ->
            spa.Options.SourcePath <- "web"

            if (app.Environment.IsDevelopment()) then
              spa.UseProxyToSpaDevelopmentServer("http://localhost:5173/"))

        app.Run()

        exitCode
