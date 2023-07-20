// SharedModels.fs
module SharedModels

open System 

type Movie =
    abstract Id: System.Guid with get
    abstract Title: string with get
    abstract Year: int with get
    abstract Cover: string with get

type MovieImpl =
    {
    Id: System.Guid
    } interface Movie with
          member this.Cover = failwith "todo"
          member this.Id = failwith "todo"
          member this.Title = failwith "todo"
          member this.Year = failwith "todo"

// type Movie = {
//     Id: System.Guid
//     Title: string
//     Year: int
//     Cover: string
// }


// The shared interface representing your client-server interaction
type MovieService = {
    helloWorld: unit -> Async<string>
    getMovies: unit -> Async<Movie list>
    getMovie: System.Guid -> Async<Movie>
    addMovie: Movie -> Async<unit>
    deleteMovie: System.Guid -> Async<unit>
}
