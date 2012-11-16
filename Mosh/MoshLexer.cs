using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mosh
{
    public class MoshLexer : ILexer
    {
        private const int IndentLength = 4;
        private int index = 0;
        private string input;
        private int indentCount = 0;
        private PQueue<Token> output;

        private Dictionary<TokenType, string> lexerRules = new Dictionary<TokenType, string>
            {
                { TokenType.CL_ACC_MOD, @"\$\+|\+\$|\+|\$" },
                { TokenType.VARNAME, @"[\w]+" },
                { TokenType.NEWLINE, "\n|\r|\r\n" },
                { TokenType.INNER_ACC_MOD, @"-|\+|\$|\$-|\$\+|-\$|\+\$" },
                { TokenType.VARTYPE, "bool|int|string" },
                { TokenType.LPAR, "[(]" },
                { TokenType.RPAR, "[)]" },
                { TokenType.VALUE, "([0-9]|[\"][\\w|\\s|?!.,]+[\"]|false|true)+" },
                { TokenType.COMMA, "[,]" },
                { TokenType.WHITESPACE, @"\s"},
                { TokenType.BLANK, @" *"},
            };

        // This SHOULD not be here - It's just for trying some stuff out
        private void TempPreprocess(ref string input)
        {
            const string tabReplacement = "    ";
            input = input.Replace("\t", tabReplacement);
        }

        public PQueue<Token> Tokenize(string input)
        {
            output = new PQueue<Token>();

            TempPreprocess(ref input);

            index = 0;
            this.input = input;
            Token nextToken;

            while ((nextToken = FetchNextToken()) != null)
            {
                if (nextToken.TokenType != TokenType.WHITESPACE)
                    output.Enqueue(nextToken);

                // If this is a new line, calculate the indentation level:
                if (nextToken.TokenType == TokenType.NEWLINE)
                    GenerateIndentDedentTokens();
            }

            return output;
        }

        private void GenerateIndentDedentTokens()
        {
            var regex = new Regex(lexerRules[TokenType.BLANK]);
            var match = regex.Match(input, index);

            if (match.Success && (match.Index - index) == 0)
            {
                var n = match.Length;

                var m = n%IndentLength;
                var N = (n - m)/IndentLength;

                if (N > indentCount)
                    GenerateIndents(N-indentCount);

                if (N < indentCount)
                    GenerateDedents(indentCount-N);
            }
        }

        private void GenerateIndents(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                indentCount++;
                output.Enqueue(new Token { TokenType = TokenType.INDENT });
            }

            index += amount * IndentLength;
        }

        private void GenerateDedents(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                indentCount--;
                output.Enqueue(new Token { TokenType = TokenType.DEDENT });
            }

            index += amount * IndentLength;
        }

        private Token FetchNextToken()
        {
            int matchLength = 0;

            if (index >= input.Length)
                return null;    

            int length = 0;
            KeyValuePair<TokenType, string>? matchingRuleValues = null;

            foreach (var rule in lexerRules)
            {
                var regex = new Regex(rule.Value);
                var match = regex.Match(input, index);

                if (match.Success && (match.Index-index) == 0)
                {
                    matchingRuleValues = rule;
                    matchLength = match.Length;
                    break;
                }
            }

            if (matchingRuleValues == null)
                throw new LexerException(string.Format("Unrecognized token found at position {0}", index+1), index+1, input);

            var matchingRule = matchingRuleValues.Value;
            var value = input.Substring(index, matchLength);
            index += matchLength;

            return new Token
                {
                    TokenType = matchingRuleValues.Value.Key,
                    Value = value
                };
        }
    }
}