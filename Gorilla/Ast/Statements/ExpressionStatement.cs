using Gorilla.Lexing;

namespace Gorilla.Ast.Statements
{
    public class ExpressionStatement : IStatement
    {
        public Token Token { get; set; }
        public IExpression Expression { get; set; }

        public string ToCode() => this.Expression?.ToCode() ?? "";

        public string TokenLiteral() => this.Token.Literal;
    }
}
