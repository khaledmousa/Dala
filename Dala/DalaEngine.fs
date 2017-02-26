module DalaEngine

open Dala
open Microsoft.FSharp.Text.Lexing
open System.Collections.Generic

type ExprResult =
    | Val of int
    | Function of string * stmt
    | None

let bag = new Dictionary<string, int>()
let funcs = new Dictionary<string, stmt>()

let ParseDala x =
    let lexbuf = LexBuffer<_>.FromString x
    let y = DalaParser.start DalaLexer.tokenize lexbuf
    y 

//Parses an expression
let rec EvalExpression expression = 
    match expression with
            | Id s -> 
                if bag.ContainsKey(s) then 
                    Val bag.[s]
                elif funcs.ContainsKey(s) then
                    Function(s, funcs.[s])
                else 
                    failwith ("Cannot evaluate identifier '" + s + "'")
            | Int i -> Val i
            | Invoc(name, param) -> EvalFunction expression//function invocation
            | EXP(e1, op, e2) -> 
                let resE1 = EvalExpression e1
                let resE2 = EvalExpression e2
                match resE1 with
                    | Val v1 -> 
                        match resE2 with
                            | Val v2 ->
                                match op with
                                    | Plus -> Val (v1 + v2)
                                    | Minus -> Val (v1 - v2)
                                    | Mult -> Val (v1 * v2)
                                    | Div -> Val (v1 / v2)
                                    | Eq -> if v1 = v2 then Val 1 else Val 0
                                    | Ne -> if v1 <> v2 then Val 1 else Val 0
                                    | Lt -> if v1 < v2 then Val 1 else Val 0
                                    | Gt -> if v1 > v2 then Val 1 else Val 0
                                    | Lte -> if v1 <= v2 then Val 1 else Val 0
                                    | Gte -> if v1 >= v2 then Val 1 else Val 0
                            |_ -> failwith("cannot evaluate function expression")                
                    |_ -> failwith("cannot evaluate function expression")
               

and Eval statement = // (bag:Dictionary<string, int>) = 
    match statement with
        | Expr(expression) -> EvalExpression expression
        | Assignment(s, e1) -> 
            let res = Eval (Expr e1)
            match res with 
                | Val num -> bag.Add(s, num)
                | Function(n, f) -> funcs.Add(s, f)            
            res
        | Fun(name, param, body) -> 
            funcs.[name] <- Fun(name, param, body)
            Function(name, funcs.[name])
        |_ -> Val -1
        
and EvalFunction expression =
    match expression with
       | Invoc(name, param) -> 
            match name with
                | Id functionName -> 
                    let f = funcs.[functionName] 
                    match f with
                        | Fun(name, paramName, body) ->                                                                
                            match EvalExpression param with
                                | Val i -> bag.[paramName] <- i
                                | Function(n, f) -> funcs.[paramName] <- f
                                | None -> failwith("Invalid function parameter " + paramName)
                            EvalFunctionBody body
                        |_ -> failwith("Not a function invocation")
                | Invoc(e1, e2) ->
                      let r1 = EvalExpression(Invoc(e1, e2))
                      match r1 with
                        | Function(n, f) -> EvalFunction(Invoc(Id n, param))
                        |_ -> failwith("invalid function invocation")
                |_ -> failwith("invalid function invocation")
       | _ -> failwith("expression is not a function invocation")

and EvalFunctionBody stmtList =
    let mutable ret = None    
    let evalStatement s = 
        match s with
            | Ret rs -> 
                ret <- EvalExpression rs
            | _ -> Eval s |> ignore
    stmtList |> List.rev |> List.iter evalStatement
    ret
    