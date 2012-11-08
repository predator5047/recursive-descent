using System;
using System.Text.RegularExpressions;

namespace Parser
{
    public static class LexerExtensions
    {
        public static bool MatchesRule(this string input, string ruleRegExp)
        {
            return Regex.IsMatch(input, ruleRegExp);
        }
    }
}