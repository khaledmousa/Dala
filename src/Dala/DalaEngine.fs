module DalaEngine

open Dala
open Microsoft.FSharp.Text.Lexing
open System.Collections.Generic

type ExprResult =
    | Val of int
    | Function of string * stmt
    | None

let mutable context = new List<Dictionary<string, int>>()
let mutable funcContext = new List<Dictionary<string, stmt>>()

let InitScope =
    //initialize global context
    context.Add(new Dictionary<string, int>())
    funcContext.Add(new Dictionary<string, stmt>())
    0

let AddScope =
    context.Add(new Dictionary<string, int>());
    funcContext.Add(new Dictionary<string, stmt>());

let RemoveScope =
    context.RemoveAt(context.Count - 1);
    funcContext.RemoveAt(funcContext.Count - 1);

let ParseDala x =
    let lexbuf = LexBuffer<_>.FromString x
    let y = DalaParser.start DalaLexer.tokenize lexbuf    
    y 

//Context retrieval
let GetScoped(key, stack:List<Dictionary<string, _>>) =

   let dicContext = stack
                    |> List.ofSeq
                    |> List.rev                    
                    |> Seq.tryFind(fun v -> v.ContainsKey(key))

   match dicContext with
        | Some d -> Option.Some d.[key]
        | _ -> Option.None
 
//Parses an expression
let rec EvalExpression expression = 
    match expression with
            | Id s -> 
                match GetScoped(s, context) with
                    | Some c -> Val c
                    | Option.None -> match GetScoped(s, funcContext) with
                        | Some f -> Function(s, f)
                        | Option.None -> failwith("Cannot evaluate identifier '" + s + "'")
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
                | Val num -> context.[context.Count - 1].Add(s, num)
                | Function(n, f) -> funcContext.[funcContext.Count - 1].Add(s, f)            
            res
        | Fun(name, param, body) ->            
            funcContext.[funcContext.Count - 1].[name] <- Fun(name, param, body)
            Function(name, funcContext.[funcContext.Count - 1].[name])        
        |_ -> Val -1
        
and EvalFunction expression =    
    match expression with
       | Invoc(name, param) ->            
            match name with
                | Id functionName when funcContext.[funcContext.Count - 1].ContainsKey(functionName) -> 
                    let f = funcContext.[funcContext.Count - 1].[functionName]                     
                    match f with
                        | Fun(name, paramName, body) ->                                                                
                            match EvalExpression param with
                                | Val i -> 
                                    AddScope
                                    context.[context.Count - 1].[paramName] <- i
                                | Function(n, f) -> 
                                    AddScope
                                    funcContext.[funcContext.Count - 1].[paramName] <- f
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
    let rec evalStatement s = 
        match s with
            | Ret rs when ret = None -> 
                ret <- EvalExpression rs
            | IfCond(expression, body) when ret = None ->
                AddScope
                match EvalExpression expression with
                    | Val v when v >= 1 -> body |> List.iter(fun s -> evalStatement s |> ignore)                    
                    | _ -> ignore 0
                RemoveScope
            | _  when ret = None -> Eval s |> ignore
            | _ -> ignore 0
    stmtList |> List.rev |> List.iter evalStatement
    RemoveScope
    ret

