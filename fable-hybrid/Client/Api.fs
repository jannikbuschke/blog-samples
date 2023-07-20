module Client.Api

open Browser
open Feliz
open Fable.Core.JsInterop

open SharedModels
open Fable.Remoting.Client

open Fable.Core

let movieService : MovieService = 
  Remoting.createApi()
  |> Remoting.buildProxy<MovieService>

let getMovies () =
    movieService.getMovies () |> Async.StartAsPromise

let getMovie id =
  movieService.getMovie id |> Async.StartAsPromise
  
let addMovie movie =
  movieService.addMovie movie |> Async.StartAsPromise
  
let deleteMovie id =
  movieService.deleteMovie id |> Async.StartAsPromise
