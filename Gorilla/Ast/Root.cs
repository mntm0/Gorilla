using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorilla.Ast
{
    public class Root : INode
    {
        public List<IStatement> Statements { get; set; }

        public string TokenLiteral()
        {
            return this.Statements.FirstOrDefault()?.TokenLiteral() ?? "";
        }

        public string ToCode()
        {
            var builder = new StringBuilder();
            foreach (var ast in this.Statements)
            {
                builder.AppendLine(ast.ToCode());
            }
            return builder.ToString().TrimEnd();
        }
    }
}
