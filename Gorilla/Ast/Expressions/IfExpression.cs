using Gorilla.Ast.Statements;
using Gorilla.Lexing;
using System.Text;

namespace Gorilla.Ast.Expressions
{
    public class IfExpression : IExpression
    {
        public Token Token { get; set; }
        public IExpression Condition { get; set; }
        public BlockStatement Consequence { get; set; }
        public BlockStatement Alternative { get; set; }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append("if");
            builder.Append(this.Condition.ToCode());
            builder.Append(" ");
            builder.Append(this.Consequence.ToCode());

            if (this.Alternative != null)
            {
                builder.Append("else ");
                builder.Append(this.Alternative.ToCode());
            }

            return builder.ToString();
        }

        public string TokenLiteral() => this.Token.Literal;
    }
}
