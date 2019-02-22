using Gorilla.Lexing;

namespace Gorilla.Ast.Expressions
{
    public class IntegerLiteral : IExpression
    {
        public Token Token { get; set; }
        public int Value { get; set; }

        public string ToCode() => this.Token.Literal;
        public string TokenLiteral() => this.Token.Literal;
    }
}
