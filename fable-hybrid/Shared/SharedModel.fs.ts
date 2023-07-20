import { int32 } from "../Client/fable_modules/fable-library-ts/Int32.js";
import { Record } from "../Client/fable_modules/fable-library-ts/Types.js";
import { IComparable, IEquatable } from "../Client/fable_modules/fable-library-ts/Util.js";
import { list_type, lambda_type, string_type, unit_type, record_type, class_type, TypeInfo } from "../Client/fable_modules/fable-library-ts/Reflection.js";

export interface Movie {
    Cover: string,
    Id: string,
    Title: string,
    Year: int32
}

export class MovieImpl extends Record implements IEquatable<MovieImpl>, IComparable<MovieImpl>, Movie {
    readonly Id: string;
    constructor(Id: string) {
        super();
        this.Id = Id;
    }
    get Cover(): string {
        throw new Error("todo");
    }
    get Id(): string {
        throw new Error("todo");
    }
    get Title(): string {
        throw new Error("todo");
    }
    get Year(): int32 {
        throw new Error("todo");
    }
}

export function MovieImpl_$reflection(): TypeInfo {
    return record_type("SharedModels.MovieImpl", [], MovieImpl, () => [["Id", class_type("System.Guid")]]);
}

export class MovieService extends Record {
    readonly helloWorld: (() => any);
    readonly getMovies: (() => any);
    readonly getMovie: ((arg0: string) => any);
    readonly addMovie: ((arg0: Movie) => any);
    readonly deleteMovie: ((arg0: string) => any);
    constructor(helloWorld: (() => any), getMovies: (() => any), getMovie: ((arg0: string) => any), addMovie: ((arg0: Movie) => any), deleteMovie: ((arg0: string) => any)) {
        super();
        this.helloWorld = helloWorld;
        this.getMovies = getMovies;
        this.getMovie = getMovie;
        this.addMovie = addMovie;
        this.deleteMovie = deleteMovie;
    }
}

export function MovieService_$reflection(): TypeInfo {
    return record_type("SharedModels.MovieService", [], MovieService, () => [["helloWorld", lambda_type(unit_type, class_type("Microsoft.FSharp.Control.FSharpAsync`1", [string_type]))], ["getMovies", lambda_type(unit_type, class_type("Microsoft.FSharp.Control.FSharpAsync`1", [list_type(class_type("SharedModels.Movie"))]))], ["getMovie", lambda_type(class_type("System.Guid"), class_type("Microsoft.FSharp.Control.FSharpAsync`1", [class_type("SharedModels.Movie")]))], ["addMovie", lambda_type(class_type("SharedModels.Movie"), class_type("Microsoft.FSharp.Control.FSharpAsync`1", [unit_type]))], ["deleteMovie", lambda_type(class_type("System.Guid"), class_type("Microsoft.FSharp.Control.FSharpAsync`1", [unit_type]))]]);
}

