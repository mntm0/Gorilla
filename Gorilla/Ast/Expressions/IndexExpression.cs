using Gorilla.Lexing;
using System.Text;

namespace Gorilla.Ast.Expressions
{
    public class IndexExpression : IExpression
    {
        public Token Token { get; set; }
        public IExpression Left { get; set; }
        public IExpression Index { get; set; }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append(this.Left.ToCode());
            builder.Append("[");
            builder.Append(this.Index.ToCode());
            builder.Append("]");
            builder.Append(")");
            return builder.ToString();
        }

        public string TokenLiteral() => this.Token.Literal;
    }
}
