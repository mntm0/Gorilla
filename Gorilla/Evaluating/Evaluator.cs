using Gorilla.Ast;
using Gorilla.Ast.Expressions;
using Gorilla.Ast.Statements;
using Gorilla.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Gorilla.Evaluating
{
    public class Evaluator
    {
        public BooleanObject True = new BooleanObject(true);
        public BooleanObject False = new BooleanObject(false);
        public NullObject Null = new NullObject();

        public IObject Eval(INode node, Enviroment enviroment)
        {
            switch (node)
            {
                // 文
                case Root root:
                    return this.EvalRootProgram(root.Statements, enviroment);
                case ExpressionStatement statement:
                    return this.Eval(statement.Expression, enviroment);
                case BlockStatement blockStatement:
                    return this.EvalBlockStatement(blockStatement, enviroment);
                case ReturnStatement returnStatement:
                    var value = this.Eval(returnStatement.ReturnValue, enviroment);
                    if (this.IsError(value)) return value;
                    return new ReturnValue(value);
                case LetStatement letStatement:
                    var letValue = this.Eval(letStatement.Value, enviroment);
                    if (this.IsError(letValue)) return letValue;
                    enviroment.Set(letStatement.Name.Value, letValue);
                    break;
                // 式
                case PrefixExpression prefixExpression:
                    var right = this.Eval(prefixExpression.Right, enviroment);
                    if (this.IsError(right)) return right;
                    return this.EvalPrefixExpression(prefixExpression.Operator, right, enviroment);
                case InfixExpression infixExpression:
                    var ifLeft = this.Eval(infixExpression.Left, enviroment);
                    if (this.IsError(ifLeft)) return ifLeft;
                    var ifRight = this.Eval(infixExpression.Right, enviroment);
                    if (this.IsError(ifRight)) return ifRight;
                    return this.EvalInfixExpression(
                        infixExpression.Operator,
                        ifLeft,
                        ifRight,
                        enviroment
                    );
                case IfExpression ifExpression:
                    return this.EvalIfExpression(ifExpression, enviroment);
                case IntegerLiteral integerLiteral:
                    return new IntegerObject(integerLiteral.Value);
                case BooleanLiteral booleanLiteral:
                    return this.ToBooleanObject(booleanLiteral.Value);
                case Identifier identifier:
                    return this.EvalIdentifier(identifier, enviroment);
                case FunctionLiteral functionLiteral:
                    return new FunctionObject()
                    {
                        Parameters = functionLiteral.Parameters,
                        Body = functionLiteral.Body,
                        Enviroment = enviroment,
                    };
                case CallExpression callExpression:
                    var fn = this.Eval(callExpression.Function, enviroment);
                    if (this.IsError(fn))
                    {
                        return fn;
                    }
                    var args = this.EvalExpressions(callExpression.Arguments, enviroment);
                    if (this.IsError(args.FirstOrDefault()))
                    {
                        return args.First();
                    }
                    return this.ApplyFunction(fn, args);
            }
            return null;
        }

        public IObject ApplyFunction(IObject obj, List<IObject> args)
        {
            var fn = obj as FunctionObject;
            if (fn == null)
            {
                return new Error($"function ではありません。: {obj?.GetType()}");
            }

            var extendedEnviroment = this.ExtendEnviroment(fn, args);
            var evaluated = this.EvalBlockStatement(fn.Body, extendedEnviroment);

            return this.UnwrapReturnValue(evaluated);
        }

        public Enviroment ExtendEnviroment(FunctionObject fn, List<IObject> args)
        {
            var enviroment = Enviroment.CreateNewEnclosedEnviroment(fn.Enviroment);

            for (int i = 0; i < fn.Parameters.Count; i++)
            {
                enviroment.Set(fn.Parameters[i].Value, args[i]);
            }

            return enviroment;
        }

        public IObject UnwrapReturnValue(IObject obj)
        {
            if (obj is ReturnValue returnValue)
            {
                return returnValue.Value;
            }
            return obj;
        }

        public List<IObject> EvalExpressions(List<IExpression> arguments, Enviroment enviroment)
        {
            var result = new List<IObject>();

            foreach (var arg in arguments)
            {
                var evaluated = this.Eval(arg, enviroment);
                if (this.IsError(evaluated))
                    return new List<IObject>() { evaluated };
                result.Add(evaluated);
            }

            return result;
        }

        public IObject EvalRootProgram(List<IStatement> statements, Enviroment enviroment)
        {
            IObject result = null;
            foreach (var statement in statements)
            {
                result = this.Eval(statement, enviroment);

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

        public IObject EvalBlockStatement(BlockStatement blockStatement, Enviroment enviroment)
        {
            IObject result = null;
            foreach (var statement in blockStatement.Statements)
            {
                result = this.Eval(statement, enviroment);

                if (result.Type() == ObjectType.RETURN_VALUE
                    || result.Type() == ObjectType.ERROR_OBJ) return result;
            }
            return result;
        }

        public IObject EvalPrefixExpression(string op, IObject right, Enviroment enviroment)
        {
            switch (op)
            {
                case "!":
                    return this.EvalBangOperator(right, enviroment);
                case "-":
                    return this.EvalMinusPrefixOperatorExpression(right, enviroment);
            }
            return new Error($"未知の演算子: {op}{right.Type()}");
        }

        public IObject EvalBangOperator(IObject right, Enviroment enviroment)
        {
            if (right == this.True) return this.False;
            if (right == this.False) return this.True;
            if (right == this.Null) return this.True;
            return this.False;
        }

        public IObject EvalMinusPrefixOperatorExpression(IObject right, Enviroment enviroment)
        {
            if (right.Type() != ObjectType.INTEGER)
                return new Error($"未知の演算子: -{right.Type()}");

            var value = (right as IntegerObject).Value;
            return new IntegerObject(-value);
        }

        public IObject EvalInfixExpression(string op, IObject left, IObject right, Enviroment enviroment)
        {
            if (left is IntegerObject leftIntegerObject
                && right is IntegerObject rightIntegerObject)
            {
                return this.EvalIntegerInfixExpression(op, leftIntegerObject, rightIntegerObject, enviroment);
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

        public IObject EvalIntegerInfixExpression(string op, IntegerObject left, IntegerObject right, Enviroment enviroment)
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

        public IObject EvalIfExpression(IfExpression ifExpression, Enviroment enviroment)
        {
            var condition = this.Eval(ifExpression.Condition, enviroment);
            if (this.IsError(condition)) return condition;

            if (this.IsTruthly(condition))
            {
                return this.EvalBlockStatement(ifExpression.Consequence, enviroment);
            }
            else if (ifExpression.Alternative != null)
            {
                return this.EvalBlockStatement(ifExpression.Alternative, enviroment);
            }
            return this.Null;
        }

        public IObject EvalIdentifier(Identifier identifier, Enviroment enviroment)
        {
            var (value, ok) = enviroment.Get(identifier.Value);
            if (ok) return value;
            return new Error($"識別子が見つかりません。: {identifier.Value}");
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
