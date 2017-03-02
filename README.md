# Dala

Dala is currently an incomplete language and interpreter. The idea to create a purely functional language where there are no variables, only single-parameter functions.

The current implementation is still in very early stages: it only supports integer values and has no real scope management (i.e. everything is globally scoped)

The language grammar uses F#'s [FsLex and FsYacc](https://fsprojects.github.io/FsLexYacc/), the interpreter is written in F#, and the REPL is in C#. 

Statements in the REPL console are evaluated after a semicolon is entered, the language itself has no semicolons yet.

## Adding two numbers 


Below is a simple example of a function that adds two numbers:

![Example add](/img/add.png)

The function `add` takes one parameter and returns another function, `addToN`, which also takes a single parameter and adds it to the parameter of `add`. So, calling `add(3)`, for example, returns a function that takes a nunmber and adds it to 3 and returns the result; so calling `add(3)(4)` passes 4 to the function returned from `add(3)` and returns 7.

## Recursion

Signle condition if-statements and recursion are also supported, as shown in the example below with a factorial function:

![Example add](/img/factorial.png)