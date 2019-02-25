using Gorilla.Ast.Statements;
using Gorilla.Lexing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorilla.Ast.Expressions
{
    public class FunctionLiteral : IExpression
    {
        public Token Token { get; set; }
        public List<Identifier> Parameters { get; set; }
        public BlockStatement Body { get; set; }

        public string ToCode()
        {
            var parameters = this.Parameters.Select(p => p.ToCode());
            var builder = new StringBuilder();
            builder.Append("fn");
            builder.Append("(");
            builder.Append(string.Join(", ", parameters));
            builder.Append(")");
            builder.Append(this.Body.ToCode());
            return builder.ToString();
        }

        public string TokenLiteral() => this.Token.Literal;
    }
}
