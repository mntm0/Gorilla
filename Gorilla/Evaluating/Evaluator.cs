using Gorilla.Ast;
using Gorilla.Ast.Expressions;
using Gorilla.Ast.Statements;
using Gorilla.Objects;
using System.Collections.Generic;

namespace Gorilla.Evaluating
{
    public class Evaluator
    {
        public BooleanObject True = new BooleanObject(true);
        public BooleanObject False = new BooleanObject(false);
        public NullObject Null = new NullObject();

        public IObject Eval(INode node)
        {
            switch (node)
            {
                //文
                case Root root:
                    return this.EvalStatements(root.Statements);
                case ExpressionStatement statement:
                    return this.Eval(statement.Expression);
                // 式
                case IntegerLiteral integerLiteral:
                    return new IntegerObject(integerLiteral.Value);
                case BooleanLiteral booleanLiteral:
                    return booleanLiteral.Value ? this.True : this.False;
            }
            return null;
        }

        public IObject EvalStatements(List<IStatement> statements)
        {
            IObject result = null;
            foreach (var statement in statements)
            {
                result = this.Eval(statement);
            }
            return result;
        }
    }
}
