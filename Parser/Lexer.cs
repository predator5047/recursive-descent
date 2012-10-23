using System.Collections.Generic;

namespace Parser
{
    public class Lexer : ILexer
    {
        public PQueue<Token> Tokenize(string input)
        {
            // Dummy data

            var q = new PQueue<Token>();

            q.Enqueue(new Token { TokenType = TokenType.IF, Value = "if" });
            q.Enqueue(new Token { TokenType = TokenType.LPAR, Value = "(" });
            q.Enqueue(new Token { TokenType = TokenType.NAME, Value = "a" });
            q.Enqueue(new Token { TokenType = TokenType.EQ, Value = "==" });
            q.Enqueue(new Token { TokenType = TokenType.VALUE, Value = "5" });
            q.Enqueue(new Token { TokenType = TokenType.RPAR, Value = ")" });
            q.Enqueue(new Token { TokenType = TokenType.LBRA, Value = "{" });
            q.Enqueue(new Token { TokenType = TokenType.NAME, Value = "Print"});
            q.Enqueue(new Token { TokenType = TokenType.LPAR, Value = "("});
            q.Enqueue(new Token { TokenType = TokenType.VALUE, Value = "\"Hello world!\""});
            q.Enqueue(new Token { TokenType = TokenType.RPAR, Value = ")"});
            q.Enqueue(new Token { TokenType = TokenType.SEMI, Value = ";" });
            q.Enqueue(new Token { TokenType = TokenType.RBRA, Value = "}" });

            return q;
        }
    }
}