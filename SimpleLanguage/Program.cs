using System;
using Parser;

namespace SimpleLanguage
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new Lexer();
            var parser = new SimpleParser(lexer);

            parser.Parse("");

            var err = parser.Errors;
            var ast = parser.AbstractSyntaxTree;
            
            // Find children
            Node<string> node;

            if (err.Count > 0)
            {
                Console.WriteLine("Parse error!");
            }
        }
    }
}
