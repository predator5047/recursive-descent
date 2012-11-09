using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parser
{
    public class Lexer : ILexer
    {
        private int index = 0;
        private string input;

        private Dictionary<TokenType, string> lexerRules = new Dictionary<TokenType, string>
            {
                { TokenType.OPER, @"\+|\-|\*|\/" },
                { TokenType.IF, "if" },
                { TokenType.LPAR, "[(]" },
                { TokenType.RPAR, "[)]" },
                { TokenType.LBRA, "[{]" },
                { TokenType.RBRA, "[}]" },
                { TokenType.EQ, @"==|<=|>=|<|>" },
                { TokenType.ASSIGN, "=" },
                { TokenType.SEMI, "[;]" },
                { TokenType.VAR, "var|bool|int|string" },
                { TokenType.VALUE, "([0-9]|[\"][\\w|\\s|?!.,]+[\"]|false|true)+" },
                { TokenType.NAME, @"[\w]+" },
                { TokenType.WHITESPACE, @"\s"},
            };

        public PQueue<Token> Tokenize(string input)
        {
            var output = new PQueue<Token>();
            index = 0;
            this.input = input;
            Token nextToken;

            while ((nextToken = FetchNextToken()) != null)
                if (nextToken.TokenType != TokenType.WHITESPACE)
                    output.Enqueue(nextToken);

            return output;
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
                //var match = Regex.Match(input.Substring(localIndex), rule.Value);
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