using Gorilla.Lexing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorilla.Ast.Expressions
{
    public class ArrayLiteral : IExpression
    {
        public Token Token { get; set; }
        public List<IExpression> Elements { get; set; }

        public string ToCode()
        {
            var elements = this.Elements.Select(e => e.ToCode());
            var builder = new StringBuilder();
            builder.Append("[");
            builder.Append(string.Join(", ", elements));
            builder.Append("]");
            return builder.ToString();
        }

        public string TokenLiteral() => this.Token.Literal;
    }
}
