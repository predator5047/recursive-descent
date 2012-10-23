using System.Collections.Generic;

namespace Parser
{
    public class FakeLexer : ILexer
    {
        public List<Token> LexerOutput { get; set; }

        public PQueue<Token> Tokenize(string input)
        {
            var queue = new PQueue<Token>();

            foreach (var token in LexerOutput)
            {
                queue.Enqueue(token);
            }

            return queue;
        }
    }
}