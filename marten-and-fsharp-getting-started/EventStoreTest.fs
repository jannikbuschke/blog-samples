module EventStoreTest.Chess

open System
open Xunit
open Marten
open Weasel.Core
open Marten.Events.Projections
open System.Text.Json.Serialization

type Color =
  | Black
  | White

type PieceType =
  | King
  | Queen
  | Bishop
  | Knight
  | Rook
  | Pawn

type ChessPiece = { Type: PieceType; Color: Color }

type Result =
  | Aborted
  | StaleMate
  | AgreedDraw
  | BlackWon
  | WhiteWon

type State =
  | NotStarted
  | Running
  | Ended of Result

type GameInitialized = { Pieces: Map<int * int, ChessPiece> }

// event
type PieceMoved = { From: int * int; To: int * int }
// event
type PieceCaptured = { Position: int * int }
// event
type GameEnded = Result

type GameEvent =
  | GameInitialized of GameInitialized
  | PieceMoved of PieceMoved
  | PieceCaptured of PieceCaptured
  | GameEnded of GameEnded

// aggregate
// As marten needs a default contstructor, apply the CLIMutable attribute
[<CLIMutable>]
type ChessGame =
  { Id: Guid
    Pieces: Map<int * int, ChessPiece>
    State: State }

  member this.Apply(e: GameEvent, meta: Marten.Events.IEvent) : ChessGame =
    match e with
    | GameInitialized e ->
      { Id = meta.StreamId
        Pieces = e.Pieces
        State = State.NotStarted }
    | PieceMoved (e: PieceMoved) ->
      let piece = this.Pieces.[e.From]
      // return a copy of the aggregate and 'change' Pieces and state
      { this with
          Pieces = this.Pieces.Add(e.To, piece).Remove (e.From)
          State = State.Running }
    | _ -> this

[<Fact>]
let ``Event store with default options should save events and build a projection`` () =
  task {
    let store =
      DocumentStore.For (fun options ->
        options.Connection (
          "User ID=postgres;Password=xxx;Host=localhost;Port=5432;Database=marten-blogpost;Pooling=true;Connection Lifetime=0;"
        )

        options.GeneratedCodeMode <- LamarCodeGeneration.TypeLoadMode.Auto
        options.AutoCreateSchemaObjects <- AutoCreate.All

        options.Projections.SelfAggregate<ChessGame> (ProjectionLifecycle.Inline)
        |> ignore

        let serializer = Marten.Services.SystemTextJsonSerializer ()

        serializer.Customize (fun v ->
          v.Converters.Add (
            JsonFSharpConverter (
              // Encode unions as a 2-valued object: unionTagName (defaults to "Case") contains the union tag, and unionFieldsName (defaults to "Fields") contains the union fields. If the case doesn't have fields, "Fields": [] is omitted. This flag is included in Default.
              JsonUnionEncoding.AdjacentTag
              // If unset, union fields are encoded as an array. If set, union fields are encoded as an object using their field names.
              ||| JsonUnionEncoding.NamedFields
              // Implicitly sets NamedFields. If set, when a union case has a single field which is a record, the fields of this record are encoded directly as fields of the object representing the union.
              ||| JsonUnionEncoding.UnwrapRecordCases
              // If set, None is represented as null, and Some x is represented the same as x. This flag is included in Default.
              ||| JsonUnionEncoding.UnwrapOption
              // If set, single-case single-field unions are serialized as the single field's value. This flag is included in Default.
              ||| JsonUnionEncoding.UnwrapSingleCaseUnions
              // In AdjacentTag and InternalTag mode, allow deserializing unions where the tag is not the first field in the JSON object.
              ||| JsonUnionEncoding.AllowUnorderedTag,
              // Also the default. Throw when deserializing a record or union where a field is null but not an explict nullable type (option, voption, skippable) https://github.com/Tarmil/FSharp.SystemTextJson/blob/master/docs/Customizing.md#allownullfields
              allowNullFields = false
            )
          ))

        options.Serializer (serializer)

      )

    store.Advanced.Clean.CompletelyRemoveAll ()
    do! store.Storage.ApplyAllConfiguredChangesToDatabaseAsync ()
    // Open a session to get access to the database
    use session = store.LightweightSession ()

    // create and id for our aggregate/stream
    let gameId = Guid.NewGuid ()
    // save our first event (GameInitialized)
    session.Events.Append (
      gameId,
      GameEvent.GameInitialized { Pieces = [ ((0, 0), { Color = Black; Type = Pawn }) ] |> Map.ofSeq }
    )
    |> ignore

    session.Events.Append (gameId, GameEvent.PieceMoved { From = (0, 0); To = (0, 1) })
    |> ignore

    // Save the event to the database. Additionally depending on our configuration marten will apply the event to any projection where this event applies to.
    let! _ = session.SaveChangesAsync ()

    // load projected view (usually latest state, bute evnetually consistent)
    let! game = session.LoadAsync<ChessGame> (gameId)

    Assert.Equal (
      ({ Id = gameId
         State = Running
         Pieces = [ ((0, 1), { Color = Black; Type = Pawn }) ] |> Map.ofSeq }),
      game
    )

    // rebuild aggregate state at version 1
    let! gameV1 = session.Events.AggregateStreamAsync<ChessGame> (gameId, 1)

    Assert.Equal (
      ({ Id = gameId
         State = NotStarted
         Pieces = [ ((0, 0), { Color = Black; Type = Pawn }) ] |> Map.ofSeq }),
      gameV1
    )

    // rebuild aggregate state at version 2
    let! gameV2 = session.Events.AggregateStreamAsync<ChessGame> (gameId, 2)

    Assert.Equal (
      ({ Id = gameId
         State = Running
         Pieces = [ ((0, 1), { Color = Black; Type = Pawn }) ] |> Map.ofSeq }),
      gameV2
    )

    // rebuild latest aggregate state (same version 2)
    let! gameV3 = session.Events.AggregateStreamAsync<ChessGame> (gameId)

    Assert.Equal (gameV2, gameV3)

    return ()
  }
