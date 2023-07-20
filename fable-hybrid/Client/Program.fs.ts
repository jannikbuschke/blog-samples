import { int32 } from "./fable_modules/fable-library-ts/Int32.js";
import { Union } from "./fable_modules/fable-library-ts/Types.js";
import { union_type, string_type, TypeInfo } from "./fable_modules/fable-library-ts/Reflection.js";
import { map } from "./fable_modules/fable-library-ts/Seq.js";
import { Interop_reactApi } from "./fable_modules/Feliz.2.6.0/Interop.fs.js";
import { TextInput } from "@mantine/core";
import { createObj } from "./fable_modules/fable-library-ts/Util.js";
import { ReactElement } from "./fable_modules/Fable.React.Types.18.3.0/Fable.React.fs.js";
import { createElement } from "react";
import React from "react";
import { startAsPromise } from "./fable_modules/fable-library-ts/Async.js";
import { singleton } from "./fable_modules/fable-library-ts/AsyncBuilder.js";
import { movieService } from "./Api.fs.js";
import { some } from "./fable_modules/fable-library-ts/Option.js";
import { FSharpList, singleton as singleton_1 } from "./fable_modules/fable-library-ts/List.js";
import { Interop_reactApi as Interop_reactApi_1 } from "./fable_modules/Feliz.2.6.0/./Interop.fs.js";
import { render } from "react-dom";

export interface IUser {
    Age: int32,
    Name: string
}

export const user: IUser = {};

user.Name = "Kaladin";

user.Age = 20;

export type Mantine_props_$union = 
    | Mantine_props<0>
    | Mantine_props<1>

export type Mantine_props_$cases = {
    0: ["Label", [string]],
    1: ["Placeholder", [string]]
}

export function Mantine_props_Label(Item: string) {
    return new Mantine_props<0>(0, [Item]);
}

export function Mantine_props_Placeholder(Item: string) {
    return new Mantine_props<1>(1, [Item]);
}

export class Mantine_props<Tag extends keyof Mantine_props_$cases> extends Union<Tag, Mantine_props_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: Mantine_props_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["Label", "Placeholder"];
    }
}

export function Mantine_props_$reflection(): TypeInfo {
    return union_type("Program.Mantine.props", [], Mantine_props, () => [[["Item", string_type]], [["Item", string_type]]]);
}

export function Mantine_TextInput(props: Iterable<Mantine_props_$union>): ReactElement {
    const props_1: Iterable<[string, any]> = map<Mantine_props_$union, [string, any]>((_arg: Mantine_props_$union): [string, any] => {
        if (_arg.tag === /* Placeholder */ 1) {
            return ["placeholder", _arg.fields[0]] as [string, any];
        }
        else {
            return ["label", _arg.fields[0]] as [string, any];
        }
    }, props);
    return Interop_reactApi.createElement(TextInput, createObj(props_1));
}

export function Counter(): ReactElement {
    startAsPromise(singleton.Delay<void>((): any => singleton.Bind<string, void>(movieService.helloWorld(), (_arg: string): any => {
        console.log(some(_arg));
        return singleton.Zero();
    })));
    const children: FSharpList<ReactElement> = singleton_1(Mantine_TextInput([Mantine_props_Label("First Name!"), Mantine_props_Placeholder("Enter your first name...")]));
    return createElement<any>("div", {
        children: Interop_reactApi_1.Children.toArray(Array.from(children)),
    });
}

render<void>(createElement(Counter, null), document.getElementById("root"));

