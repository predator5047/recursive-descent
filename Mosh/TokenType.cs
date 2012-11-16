namespace Mosh
{
    public enum TokenType
    {
        CL_ACC_MOD,
        VARNAME,
        NEWLINE,
        INNER_ACC_MOD,
        VARTYPE,
        LPAR,
        RPAR,
        INDENT,
        DEDENT,
        VALUE,
        COMMA,
        BLANK,

        // Internal tokens
        NEW_RULE,
        WHITESPACE,
    }
}