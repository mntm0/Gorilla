using System;

namespace Gorilla
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Gorilla Script!");

            var repl = new Repl();
            repl.Start();
        }
    }
}
