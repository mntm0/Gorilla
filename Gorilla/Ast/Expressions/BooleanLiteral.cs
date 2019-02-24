using Gorilla.Lexing;

namespace Gorilla.Ast.Expressions
{
    public class BooleanLiteral : IExpression
    {
        public Token Token { get; set; }
        public bool Value { get; set; }

        public string ToCode() => this.Token.Literal;
        public string TokenLiteral() => this.Token.Literal;
    }
}
