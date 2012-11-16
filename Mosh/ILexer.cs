namespace Mosh
{
    public interface ILexer
    {
        PQueue<Token> Tokenize(string input);
    }
}