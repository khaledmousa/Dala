module Dala

type operator = Plus | Minus | Mult | Div | Eq | Ne | Lt | Lte | Gt | Gte
type expr = 
        | Int of int 
        | Id of string
        | EXP of expr * operator * expr
        | Invoc of expr * expr      

type stmt =
        | Expr of expr
        | Assignment of string * expr  
        | Fun of string * string * stmt list
        | IfCond of expr * stmt list
        | Ret of expr