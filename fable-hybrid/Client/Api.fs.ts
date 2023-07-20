import { Remoting_buildProxy_64DC51C } from "./fable_modules/Fable.Remoting.Client.7.25.0/./Remoting.fs.js";
import { RemotingModule_createApi } from "./fable_modules/Fable.Remoting.Client.7.25.0/Remoting.fs.js";
import { Movie, MovieService, MovieService_$reflection } from "../Shared/SharedModel.fs.js";
import { startAsPromise } from "./fable_modules/fable-library-ts/Async.js";
import { FSharpList } from "./fable_modules/fable-library-ts/List.js";

export const movieService: MovieService = Remoting_buildProxy_64DC51C<MovieService>(RemotingModule_createApi(), MovieService_$reflection());

export function getMovies(): Promise<FSharpList<Movie>> {
    return startAsPromise(movieService.getMovies());
}

export function getMovie(id: string): Promise<Movie> {
    return startAsPromise(movieService.getMovie(id));
}

export function addMovie(movie: Movie): Promise<void> {
    return startAsPromise(movieService.addMovie(movie));
}

export function deleteMovie(id: string): Promise<void> {
    return startAsPromise(movieService.deleteMovie(id));
}

