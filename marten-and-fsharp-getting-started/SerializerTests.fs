module EventStoreTest.Serializer

open Xunit
open System.Text.Json.Serialization

type MyRecord = { Foo: string }

type Du =
  | Case1
  | Case2 of int
  | Case3 of int * string
  | Case4 of name: string
  | Case5 of name: string * payload: MyRecord
  | Case6 of MyRecord
  | Case7 of Foo: string

type Id = | Id of System.Guid

type NoFields =
  | Case1
  | Case2

type RecordWithNoFieldDu = { Field: NoFields }

let defaultOptions = System.Text.Json.JsonSerializerOptions ()
defaultOptions.Converters.Add (JsonFSharpConverter ())

let defaultSerialize value =
  System.Text.Json.JsonSerializer.Serialize (value, defaultOptions)

let defaultDeserialize<'T> (value: string) =
  System.Text.Json.JsonSerializer.Deserialize<'T> (value, defaultOptions)

[<Fact>]
let ``Default settings json format`` () =
  Assert.Equal ("[1,2,3]", defaultSerialize [ 1; 2; 3 ])
  Assert.Equal ("""[1,"hello"]""", defaultSerialize (1, "hello"))
  Assert.Equal ("null", defaultSerialize None)
  Assert.Equal ("5", defaultSerialize (Some 5))
  Assert.Equal ("""{"Case":"Case1"}""", defaultSerialize Du.Case1)
  Assert.Equal ("""{"Case":"Case2","Fields":[2]}""", defaultSerialize (Du.Case2 2))
  Assert.Equal ("""{"Case":"Case3","Fields":[5,"hello"]}""", defaultSerialize (Du.Case3 (5, "hello")))
  Assert.Equal ("""{"Case":"Case4","Fields":["Name"]}""", defaultSerialize (Du.Case4 ("Name")))

  Assert.Equal (
    """{"Case":"Case5","Fields":["Hello",{"Foo":"Hello"}]}""",
    defaultSerialize (Du.Case5 ("Hello", { Foo = "Hello" }))
  )

  Assert.Equal ("""{"Case":"Case6","Fields":[{"Foo":"Hello"}]}""", defaultSerialize (Du.Case6 ({ Foo = "Hello" })))

  Assert.Equal (
    "\"7817a4bd-c7c3-460f-9679-aa10afd4ac89\"",
    defaultSerialize (Id (System.Guid.Parse ("7817a4bd-c7c3-460f-9679-aa10afd4ac89")))
  )

  Assert.Equal ("""{"Case":"Case1"}""", defaultSerialize NoFields.Case1)
  Assert.Equal ("""{"Field":{"Case":"Case1"}}""", defaultSerialize { Field = NoFields.Case1 })
  // a little weird, the default allows to serialize records with null values...
  Assert.Equal ("""{"Foo":null}""", defaultSerialize { Foo = null })
  // but throws on deserialization
  Assert.Throws<System.Text.Json.JsonException> (fun () -> defaultDeserialize<MyRecord> """{"Foo":null}""" |> ignore)


let customOptions = System.Text.Json.JsonSerializerOptions ()

customOptions.Converters.Add (
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
    // If set, the field of a single-field union case is encoded as just the value rather than a single-value array or object.
    // ||| JsonUnionEncoding.UnwrapSingleFieldCases
    // If set, union cases that don't have fields are encoded as a bare string.
    // ||| JsonUnionEncoding.UnwrapFieldlessTags
    // In AdjacentTag and InternalTag mode, allow deserializing unions where the tag is not the first field in the JSON object.
    ||| JsonUnionEncoding.AllowUnorderedTag,

    // Also the default. Throw when deserializing a record or union where a field is null but not an explict nullable type (option, voption, skippable) https://github.com/Tarmil/FSharp.SystemTextJson/blob/master/docs/Customizing.md#allownullfields
    allowNullFields = false
  )
)

let customSerialize (value) =
  System.Text.Json.JsonSerializer.Serialize (value, customOptions)

let customDeserialize<'T> (value: string) =
  System.Text.Json.JsonSerializer.Deserialize<'T> (value, customOptions)

[<Fact>]
let ``Custom settings json format`` () =

  Assert.Equal ("[1,2,3]", customSerialize [ 1; 2; 3 ])
  Assert.Equal ("""[1,"hello"]""", customSerialize (1, "hello"))
  Assert.Equal ("null", customSerialize None)
  Assert.Equal ("5", customSerialize (Some 5))
  Assert.Equal ("""{"Case":"Case1"}""", customSerialize Du.Case1)
  Assert.Equal ("""{"Case":"Case2","Fields":{"Item":2}}""", customSerialize (Du.Case2 2))
  Assert.Equal ("""{"Case":"Case3","Fields":{"Item1":5,"Item2":"hello"}}""", customSerialize (Du.Case3 (5, "hello")))

  Assert.Equal ("""{"Case":"Case4","Fields":{"name":"Hello"}}""", customSerialize (Du.Case4 ("Hello")))

  Assert.Equal (
    """{"Case":"Case5","Fields":{"name":"Hello","payload":{"Foo":"Hello"}}}""",
    customSerialize (Du.Case5 ("Hello", { Foo = "Hello" }))
  )

  Assert.Equal ("""{"Case":"Case6","Fields":{"Foo":"Hello"}}""", customSerialize (Du.Case6 ({ Foo = "Hello" })))
  Assert.Equal ("""{"Case":"Case7","Fields":{"Foo":"Hello"}}""", customSerialize (Du.Case7 ("Hello")))

  Assert.Equal (
    "\"7817a4bd-c7c3-460f-9679-aa10afd4ac89\"",
    customSerialize (Id (System.Guid.Parse ("7817a4bd-c7c3-460f-9679-aa10afd4ac89")))
  )

  Assert.Equal ("""{"Case":"Case1"}""", customSerialize NoFields.Case1)
  // Assert.Equal ("""{"Field":{"Case":"Case1"}}""", customSerialize { Field = NoFields.Case1 })
  Assert.Equal ("""{"Field":{"Case":"Case1"}}""", customSerialize { Field = NoFields.Case1 })
  Assert.Equal ("""{"Foo":null}""", defaultSerialize { Foo = null })
  Assert.Throws<System.Text.Json.JsonException> (fun () -> defaultDeserialize<MyRecord> """{"Foo":null}""" |> ignore)
