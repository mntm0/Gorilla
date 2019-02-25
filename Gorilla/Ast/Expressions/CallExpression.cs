using Gorilla.Lexing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorilla.Ast.Expressions
{
    public class CallExpression : IExpression
    {
        public Token Token { get; set; }
        public IExpression Function { get; set; }
        public List<IExpression> Arguments { get; set; }

        public string ToCode()
        {
            var args = this.Arguments.Select(a => a.ToCode());
            var builder = new StringBuilder();
            builder.Append(this.Function.ToCode());
            builder.Append("(");
            builder.Append(string.Join(", ", args));
            builder.Append(")");
            return builder.ToString();
        }

        public string TokenLiteral() => this.Token.Literal;
    }
}
