using Gorilla.Ast.Expressions;
using Gorilla.Lexing;
using System.Text;

namespace Gorilla.Ast.Statements
{
    public class LetStatement : IStatement
    {
        public Token Token { get; set; }
        public Identifier Name { get; set; }
        public IExpression Value { get; set; }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append(this.Token?.Literal ?? "");
            builder.Append(" ");
            builder.Append(this.Name?.ToCode() ?? "");
            builder.Append(" = ");
            builder.Append(this.Value?.ToCode() ?? "");
            builder.Append(";");
            return builder.ToString();
        }

        public string TokenLiteral() => this.Token.Literal;
    }
}
