import { Formik } from "formik";
import { useQuery } from "react-query";
import { Movie } from "../../Shared/SharedModel.fs.js";
import { movieService, getMovies, addMovie } from "../Api.fs.js";
import { toArray } from "../fable_modules/fable-library-ts/List.js";
import { BadgeCard } from "./movie-card";

export function MovieCardGrid() {
  console.log({ movieService });

  const { data, isLoading, error } = useQuery("movies", getMovies, {
    select: (data) => toArray(data),
  });
  //   return <div>FOOO</div>;
  console.log({ data, isLoading, error });
  if (!data) {
    return <div>Loading...</div>;
  }
  return (
    <div>
      <Formik
        initialValues={{ id: "", title: "", year: 0, cover: "" }}
        onSubmit={({ id, title, year, cover }) => {
          addMovie(new Movie(id, title, year, cover));
        }}
      >
        {(f) => <form></form>}
      </Formik>

      {data.map((v) => (
        <BadgeCard image={""} title={v.Title} />
        // <div>{v.Title}</div>
      ))}
      {JSON.stringify({ data, isLoading, error })}
    </div>
  );
}
