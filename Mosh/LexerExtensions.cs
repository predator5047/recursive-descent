using System.Text.RegularExpressions;

namespace Mosh
{
    public static class LexerExtensions
    {
        public static bool MatchesRule(this string input, string ruleRegExp)
        {
            return Regex.IsMatch(input, ruleRegExp);
        }
    }
}