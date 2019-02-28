using Gorilla.Evaluating;
using Gorilla.Lexing;
using Gorilla.Objects;
using Gorilla.Parsing;
using System;

namespace Gorilla
{
    public class Repl
    {
        const string PROMPT = ">> ";

        public void Start()
        {
            var enviroment = new Enviroment();
            while (true)
            {
                Console.Write(PROMPT);

                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) return;

                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();

                if (parser.Errors.Count > 0)
                {
                    foreach (var error in parser.Errors)
                    {
                        Console.WriteLine($"\t{error}");
                    }
                    continue;
                }

                var evaluator = new Evaluator();
                var evaluated = evaluator.Eval(root, enviroment);
                if (evaluated != null)
                {
                    Console.WriteLine(evaluated.Inspect());
                }
            }
        }
    }
}
