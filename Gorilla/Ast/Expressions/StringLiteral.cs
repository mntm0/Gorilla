using Gorilla.Lexing;

namespace Gorilla.Ast.Expressions
{
    public class StringLiteral : IExpression
    {
        public Token Token { get; set; }
        public string Value { get; set; }

        public string ToCode() => $"\"{this.Value}\"";
        public string TokenLiteral() => this.Token.Literal;
    }
}
