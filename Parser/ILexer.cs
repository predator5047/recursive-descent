using System.Collections.Generic;

namespace Parser
{
    public interface ILexer
    {
        PQueue<Token> Tokenize(string input);
    }
}