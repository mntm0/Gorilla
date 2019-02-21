﻿using Gorilla.Lexing;
using System.Text;

namespace Gorilla.Ast.Statements
{
    public class ReturnStatement: IStatement
    {
        public Token Token { get; set; }
        public IExpression ReturnValue { get; set; }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append(this.Token?.Literal ?? "");
            builder.Append(" ");
            builder.Append(this.ReturnValue?.ToCode() ?? "");
            builder.Append(";");
            return builder.ToString();
        }

        public string TokenLiteral() => this.Token.Literal;
    }
}
