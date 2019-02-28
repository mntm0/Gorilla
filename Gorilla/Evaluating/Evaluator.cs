using Gorilla.Ast;
using Gorilla.Ast.Expressions;
using Gorilla.Ast.Statements;
using Gorilla.Objects;
using System;
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
                    return this.EvalStatements(root.Statements);
                case ExpressionStatement statement:
                    return this.Eval(statement.Expression);
                case BlockStatement blockStatement:
                    return this.EvalStatements(blockStatement.Statements);
                // 式
                case PrefixExpression prefixExpression:
                    var right = this.Eval(prefixExpression.Right);
                    return this.EvalPrefixExpression(prefixExpression.Operator, right);
                case InfixExpression infixExpression:
                    return this.EvalInfixExpression(
                        infixExpression.Operator,
                        this.Eval(infixExpression.Left),
                        this.Eval(infixExpression.Right)
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

        public IObject EvalStatements(List<IStatement> statements)
        {
            IObject result = null;
            foreach (var statement in statements)
            {
                result = this.Eval(statement);
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
            return this.Null;
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
            if (right.Type() != ObjectType.INTEGER) return this.Null;

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

            return this.Null;
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

            if (this.IsTruthly(condition))
            {
                return this.EvalStatements(ifExpression.Consequence.Statements);
            }
            else if (ifExpression.Alternative != null)
            {
                return this.EvalStatements(ifExpression.Alternative.Statements);
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
    }
}
