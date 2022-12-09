module Tests

open Xunit
open Marten
open Weasel.Core

type Du =
  | Case1 of int
  | Case2

type SampleType =
  { Id: string
    Hello: string
    Option: int option
    Du: Du }

[<Fact>]
let ``Document store with default serializer should save record`` () =
  task {
    let store =
      DocumentStore.For (fun options ->
        options.Connection (
          "User ID=postgres;Password=xxx;Host=localhost;Port=5432;Database=marten-blogpost;Pooling=true;Connection Lifetime=0;"
        )

        options.AutoCreateSchemaObjects <- AutoCreate.All)

    use session = store.OpenSession ()

    session.Store (
      { Id = "1"
        Hello = "World"
        Option = Some 5
        Du = Case1 2 }
    )

    let! _ = session.SaveChangesAsync ()

    use session = store.OpenSession ()
    let! sample = session.LoadAsync<SampleType> "1"

    Assert.Equal (
      sample,
      ({ Id = "1"
         Hello = "World"
         Option = Some 5
         Du = Case1 2 })
    )

    return ()
  }
