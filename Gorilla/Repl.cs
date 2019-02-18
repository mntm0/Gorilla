using Gorilla.Lexing;
using System;

namespace Gorilla
{
    public class Repl
    {
        const string PROMPT = ">> ";

        public void Start()
        {
            while (true)
            {
                Console.Write(PROMPT);

                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) return;

                var lexer = new Lexer(input);
                for (var token = lexer.NextToken(); token.Type != TokenType.EOF; token = lexer.NextToken())
                {
                    Console.WriteLine($"{{ Type: {token.Type.ToString()}, Literal: {token.Literal} }}");
                }
            }
        }
    }
}
