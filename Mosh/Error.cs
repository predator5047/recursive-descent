namespace Mosh
{
    public class Error
    {
        public string Message { get; set; }
        public ErrorType Type { get; set; }

        public override string ToString()
        {
            return string.Format(Message);
        }
    }
}