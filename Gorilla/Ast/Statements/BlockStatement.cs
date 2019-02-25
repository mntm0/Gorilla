using Gorilla.Lexing;
using System.Collections.Generic;
using System.Text;

namespace Gorilla.Ast.Statements
{
    public class BlockStatement : IStatement
    {
        public Token Token { get; set; }
        public List<IStatement> Statements { get; set; }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append("{");
            foreach (var statement in this.Statements)
            {
                builder.Append(statement.ToCode() + " ");
            }
            builder.Append("}");
            return builder.ToString();
        }

        public string TokenLiteral() => this.Token.Literal;
    }
}
