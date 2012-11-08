namespace Parser
{
    public enum TokenType
    {
        IF,
        EQ,
        LPAR,
        RPAR,
        LBRA,
        RBRA,
        VAR,
        NAME,
        ASSIGN,
        VALUE,
        SEMI,

        // Internal tokens
        NEW_RULE,
        WHITESPACE,
    }
}