using Gorilla.Lexing;
using System.Text;

namespace Gorilla.Ast.Expressions
{
    public class InfixExpression : IExpression
    {
        public Token Token { get; set; }
        public IExpression Left { get; set; }
        public string Operator { get; set; }
        public IExpression Right { get; set; }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append(this.Left.ToCode());
            builder.Append(" ");
            builder.Append(this.Operator);
            builder.Append(" ");
            builder.Append(this.Right.ToCode());
            builder.Append(")");
            return builder.ToString();
        }
        public string TokenLiteral() => this.Token.Literal;
    }
}
