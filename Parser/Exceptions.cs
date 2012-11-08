using System;

namespace Parser
{
    public class LexerException : Exception
    {
        public int Position { get; private set; }
        public string Input { get; set; }

        public LexerException(string message, int position, string input) : base(message)
        {
            Position = position;
            Input = input;
        }
    }
}