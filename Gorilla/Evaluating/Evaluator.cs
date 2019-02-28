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
                // 文
                case Root root:
                    return this.EvalRootProgram(root.Statements);
                case ExpressionStatement statement:
                    return this.Eval(statement.Expression);
                case BlockStatement blockStatement:
                    return this.EvalBlockStatement(blockStatement);
                case ReturnStatement returnStatement:
                    var value = this.Eval(returnStatement.ReturnValue);
                    if (this.IsError(value)) return value;
                    return new ReturnValue(value);
                // 式
                case PrefixExpression prefixExpression:
                    var right = this.Eval(prefixExpression.Right);
                    if (this.IsError(right)) return right;
                    return this.EvalPrefixExpression(prefixExpression.Operator, right);
                case InfixExpression infixExpression:
                    var ifLeft = this.Eval(infixExpression.Left);
                    if (this.IsError(ifLeft)) return ifLeft;
                    var ifRight = this.Eval(infixExpression.Right);
                    if (this.IsError(ifRight)) return ifRight;
                    return this.EvalInfixExpression(
                        infixExpression.Operator,
                        ifLeft,
                        ifRight
                    );
                case IfExpression ifExpression:
                    return this.EvalIfExpression(ifExpression);
                case IntegerLiteral integerLiteral:
                    return new IntegerObject(integerLiteral.Value);
                case BooleanLiteral booleanLiteral:
                    return this.ToBooleanObject(booleanLiteral.Value);
            }
            return null;
        }

        public IObject EvalRootProgram(List<IStatement> statements)
        {
            IObject result = null;
            foreach (var statement in statements)
            {
                result = this.Eval(statement);

                switch (result)
                {
                    case ReturnValue returnValue:
                       return returnValue.Value;
                    case Error _:
                        return result;
                    default:
                        break;
                }
            }
            return result;
        }

        public IObject EvalBlockStatement(BlockStatement blockStatement)
        {
            IObject result = null;
            foreach (var statement in blockStatement.Statements)
            {
                result = this.Eval(statement);

                if (result.Type() == ObjectType.RETURN_VALUE
                    || result.Type() == ObjectType.ERROR_OBJ) return result;
            }
            return result;
        }

        public IObject EvalPrefixExpression(string op, IObject right)
        {
            switch (op)
            {
                case "!":
                    return this.EvalBangOperator(right);
                case "-":
                    return this.EvalMinusPrefixOperatorExpression(right);
            }
            return new Error($"未知の演算子: {op}{right.Type()}");
        }

        public IObject EvalBangOperator(IObject right)
        {
            if (right == this.True) return this.False;
            if (right == this.False) return this.True;
            if (right == this.Null) return this.True;
            return this.False;
        }

        public IObject EvalMinusPrefixOperatorExpression(IObject right)
        {
            if (right.Type() != ObjectType.INTEGER)
                return new Error($"未知の演算子: -{right.Type()}");

            var value = (right as IntegerObject).Value;
            return new IntegerObject(-value);
        }

        public IObject EvalInfixExpression(string op, IObject left, IObject right)
        {
            if (left is IntegerObject leftIntegerObject
                && right is IntegerObject rightIntegerObject)
            {
                return this.EvalIntegerInfixExpression(op, leftIntegerObject, rightIntegerObject);
            }

            switch (op)
            {
                case "==":
                    return ToBooleanObject(left == right);
                case "!=":
                    return ToBooleanObject(left!= right);
            }

            if (left.Type() != right.Type())
                return new Error($"型のミスマッチ: {left.Type()} {op} {right.Type()}");

            return new Error($"未知の演算子: {left.Type()} {op} {right.Type()}");
        }

        public IObject EvalIntegerInfixExpression(string op, IntegerObject left, IntegerObject right)
        {
            var leftValue = left.Value;
            var rightValue = right.Value;

            switch (op)
            {
                case "+":
                    return new IntegerObject(leftValue + rightValue);
                case "-":
                    return new IntegerObject(leftValue - rightValue);
                case "*":
                    return new IntegerObject(leftValue * rightValue);
                case "/":
                    return new IntegerObject(leftValue / rightValue);
                case "<":
                    return this.ToBooleanObject(leftValue < rightValue);
                case ">":
                    return this.ToBooleanObject(leftValue > rightValue);
                case "==":
                    return this.ToBooleanObject(leftValue == rightValue);
                case "!=":
                    return this.ToBooleanObject(leftValue != rightValue);
            }
            return this.Null;
        }

        public BooleanObject ToBooleanObject(bool value) => value ? this.True : this.False;

        public IObject EvalIfExpression(IfExpression ifExpression)
        {
            var condition = this.Eval(ifExpression.Condition);
            if (this.IsError(condition)) return condition;

            if (this.IsTruthly(condition))
            {
                return this.EvalBlockStatement(ifExpression.Consequence);
            }
            else if (ifExpression.Alternative != null)
            {
                return this.EvalBlockStatement(ifExpression.Alternative);
            }
            return this.Null;
        }

        public bool IsTruthly(IObject obj)
        {
            if (obj == this.True) return true;
            if (obj == this.False) return false;
            if (obj == this.Null) return false;
            return true;
        }

        public bool IsError(IObject obj)
        {
            if (obj != null) return obj.Type() == ObjectType.ERROR_OBJ;
            return false;
        }
    }
}
