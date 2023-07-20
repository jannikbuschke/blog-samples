open Browser
open Feliz
open Fable.Core.JsInterop

open Fable.Core


open Fable.Remoting.Client.Remoting
open Fable.Remoting.Client.Proxy
open Fable.Remoting.Client.BrowserFileExtensions
open Fable.Remoting.Client.Http

open SharedModels
open Fable.Remoting.Client
open Client.Api

type IUser =
    abstract Name: string with get, set
    abstract Age: int with get, set

let user = createEmpty<IUser>
user.Name <- "Kaladin"
user.Age <- 20



module Mantine =
    type props =
        | Label of string
        | Placeholder of string

    let TextInput (props: props seq) =
        let props =
            props
            |> Seq.map (function
                | Label label -> "label" ==> label
                | Placeholder placeholder -> "placeholder" ==> placeholder)

        Interop.reactApi.createElement (import "TextInput" "@mantine/core", createObj props)

open Mantine

[<ReactComponent>]
let Counter () =

    async {
        let! message = movieService.helloWorld ()
        console.log (message)

    }
    |> Async.StartAsPromise


    Html.div [ TextInput [ props.Label "First Name!"
                           props.Placeholder "Enter your first name..." ] ]


ReactDOM.render (Counter(), document.getElementById "root")
