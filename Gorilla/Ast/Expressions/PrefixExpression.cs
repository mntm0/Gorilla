using Gorilla.Lexing;

namespace Gorilla.Ast.Expressions
{
    public class PrefixExpression : IExpression
    {
        public Token Token { get; set; }
        public string Operator { get; set; }
        public IExpression Right { get; set; }

        public string ToCode() => $"({this.Operator}{this.Right.ToCode()})";
        public string TokenLiteral() => this.Token.Literal;
    }
}
