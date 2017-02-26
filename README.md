# Dala

Dala is currently an incomplete language and interpreter. The idea to create a purely functional language where there are no variables, only single-parameter functions.

The current implementation is still in very early stages: it only supports integer values and has no real scope management (i.e. everything is globally scoped)

The language grammar uses F#'s [FsLex and FsYacc](https://fsprojects.github.io/FsLexYacc/), the interpreter is written in F#, and the REPL is in C#. 

Statements in the REPL console are evaluated after a semicolon is entered, the language itself has no semicolons yet.

Below is a simple example of an function that adds two numbers:

![Example add](https://github.com/khaledmousa/Dala/tree/master/img/add.png)