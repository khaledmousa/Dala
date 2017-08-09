using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalar
{
    class Program
    {
        static void Main(string[] args)
        {
            var _ = DalaEngine.InitScope;
            var input = string.Empty;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to the Dala REPL!");
            Console.ResetColor();
            Console.WriteLine("Type 'q' to exit");
            var statement = new StringBuilder();
            while (true) //REPL
            {
                try
                {
                    Console.Write(">");
                    input = Console.ReadLine();
                    if (input.Trim().ToLower() == "q") break;

                    if (input.Trim().EndsWith(";"))
                    {
                        if (input.Trim() != ";") statement.Append(input.Substring(0, input.Length - 1));
                        input = statement.ToString();
                        statement.Clear();
                    }
                    else
                    {
                        statement.Append(input);
                        continue;
                    }

                    var exp = DalaEngine.ParseDala(input);
                    var evalResult = DalaEngine.Eval(exp);
                    if (evalResult.IsFunction)
                    {
                        var f = ((Dala.stmt.Fun)((DalaEngine.ExprResult.Function)evalResult).Item2);
                        Console.WriteLine("func " + f.Item1 + "(" + f.Item2 + ")");
                    }
                    else
                    {
                        var i = (DalaEngine.ExprResult.Val)evalResult;
                        Console.WriteLine(i.Item);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }
    }
}
