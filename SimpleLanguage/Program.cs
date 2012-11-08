using System;
using Parser;

namespace SimpleLanguage
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new Lexer();

            Console.WriteLine("Lexer console");
            Console.WriteLine("-------------");
            Console.WriteLine();
            
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                try
                {
                    var tokens = lexer.Tokenize(input);
                    int i = 0;

                    Console.WriteLine(" -- OUTPUT");
                    foreach (var token in tokens)
                    {
                        Console.WriteLine(" -- {0}. {1}\t{2}", ++i, token.TokenType, token.Value);
                    }
                }
                catch (LexerException ex)
                {
                    Console.SetCursorPosition(ex.Position+1, Console.CursorTop);
                    var c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("^");
                    Console.ForegroundColor = c;
                    Console.Write(ex.Message);
                    Console.WriteLine();
                }
            }
        }
    }
}
