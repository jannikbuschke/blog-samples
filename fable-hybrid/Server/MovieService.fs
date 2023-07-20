module Server.HelloWorldService

open System
open System.Collections.Generic
open SharedModels

let database = Dictionary<System.Guid, Movie>()

let addMovie (movie: Movie) =
    database.Add(movie.Id,movie)

// addMovie({
//     new Movie with
//         Id = System.Guid.NewGuid()
//         Title = "The Matrix"
//         Year = 1999
//         Cover=""
// })
// addMovie({
//     Id = System.Guid.NewGuid()
//     Title = "Old Boy"
//     Year = 2003
//     Cover = ""
// })

let service : MovieService = {
    helloWorld = fun () -> async { return "Hello From Fable.Remoting" }
    getMovies = fun () -> async { return database.Values |> Seq.toList }
    getMovie = fun (id: System.Guid) -> async { return database.[id] }
    addMovie = fun (movie: Movie) -> async { addMovie(movie) }
    deleteMovie = fun (id: System.Guid) -> async { database.Remove(id) }
}
