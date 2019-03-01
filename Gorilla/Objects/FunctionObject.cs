using Gorilla.Ast.Expressions;
using Gorilla.Ast.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorilla.Objects
{
    public class FunctionObject : IObject
    {
        public List<Identifier> Parameters { get; set; } = new List<Identifier>();
        public BlockStatement Body { get; set; }
        public Enviroment Enviroment { get; set; }

        public string Inspect()
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

        public ObjectType Type() => ObjectType.FUNCTION_OBJ;
    }
}
