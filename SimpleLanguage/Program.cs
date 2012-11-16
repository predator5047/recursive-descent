using System;
using Mosh;

namespace SimpleLanguage
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new MoshLexer();

            Console.WriteLine("Lexer console");
            Console.WriteLine("-------------");
            Console.WriteLine();
            
            while (true)
            {
                var input = string.Empty;
                var row = string.Empty;

                while (row != ";")
                {
                    row = Console.ReadLine();

                    if (row != ";")
                    {
                        row += '\n';

                        if (input == string.Empty)
                            input = row;
                        else
                            input += row;
                    }
                }

                try
                {
                    var tokens = lexer.Tokenize(input);
                    int i = 0;

                    Console.WriteLine(" -- OUTPUT");
                    foreach (var token in tokens)
                    {
                        Console.WriteLine(" -- {0}. {1}\t\t{2}", ++i, token.TokenType, token.Value);
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
