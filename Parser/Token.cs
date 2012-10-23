namespace Parser
{
    public class Token
    {
        public TokenType TokenType;
        public string Value;

        public override string ToString()
        {
            return string.Format("{0}\t{1}", TokenType, Value);
        }
    }
}