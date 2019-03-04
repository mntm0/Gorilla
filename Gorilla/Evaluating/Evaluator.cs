﻿using Gorilla.Ast;
using Gorilla.Ast.Expressions;
using Gorilla.Ast.Statements;
using Gorilla.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gorilla.Evaluating
{
    public class Evaluator
    {
        public static BooleanObject True = new BooleanObject(true);
        public static BooleanObject False = new BooleanObject(false);
        public static NullObject Null = new NullObject();

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
                case StringLiteral stringLiteral:
                    return new StringObject(stringLiteral.Value);
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
                case ArrayLiteral arrayLiteral:
                    var elements = this.EvalExpressions(arrayLiteral.Elements, enviroment);
                    if (elements.Count == 1 && this.IsError(elements[0]))
                    {
                        return elements[0];
                    }
                    return new ArrayObject(elements);
                case IndexExpression indexExpression:
                    var left = this.Eval(indexExpression.Left, enviroment);
                    if (this.IsError(left)) return left;
                    var index = this.Eval(indexExpression.Index, enviroment);
                    if (this.IsError(index)) return index;
                    return this.EvalIndexExpression(left, index);
            }
            return null;
        }

        public IObject EvalIndexExpression(IObject left, IObject index)
        {
            var array = left as ArrayObject;
            var indexInt = index as IntegerObject;
            if (array == null)
            {
                return new Error($"インデックス演算子は {left.Type()} をサポートしません。");
            }
            else if (indexInt == null)
            {
                return new Error($"インデックスが整数型ではありません。({index.Type()})");
            }

            return this.EvalArrayIndexExpression(array, indexInt);
        }

        public IObject EvalArrayIndexExpression(ArrayObject array, IntegerObject index)
        {
            var i = index.Value;
            var max = array.Elements.Count - 1;
            if (i < 0 || max < i) return Evaluator.Null;

            return array.Elements[i];
        }

        public IObject ApplyFunction(IObject obj, List<IObject> args)
        {
            switch (obj)
            {
                case FunctionObject fn:
                    var extendedEnviroment = this.ExtendEnviroment(fn, args);
                    var evaluated = this.EvalBlockStatement(fn.Body, extendedEnviroment);
                    return this.UnwrapReturnValue(evaluated);
                case BuiltinFunctionObject fn:
                    return fn.Function(args);
                default:
                    return new Error($"function ではありません。: {obj?.GetType()}");
            }
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
            if (right == Evaluator.True) return Evaluator.False;
            if (right == Evaluator.False) return Evaluator.True;
            if (right == Evaluator.Null) return Evaluator.True;
            return Evaluator.False;
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

            if (left is StringObject leftStringObject
                && right is StringObject rightStringObject)
            {
                return this.EvalStringInfixExpression(op, leftStringObject, rightStringObject, enviroment);
            }

            switch (op)
            {
                case "==":
                    return ToBooleanObject(left == right);
                case "!=":
                    return ToBooleanObject(left != right);
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
            return Evaluator.Null;
        }

        public IObject EvalStringInfixExpression(string op, StringObject left, StringObject right, Enviroment enviroment)
        {
            var leftValue = left.Value;
            var rightValue = right.Value;

            switch (op)
            {
                case "+":
                    return new StringObject(leftValue + rightValue);
                default:
                    return new Error($"未知の演算子: {left.Type()} {op} {right.Type()}");
            }
        }

        public BooleanObject ToBooleanObject(bool value) => value ? Evaluator.True : Evaluator.False;

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
            return Evaluator.Null;
        }

        public IObject EvalIdentifier(Identifier identifier, Enviroment enviroment)
        {
            var (value, ok) = enviroment.Get(identifier.Value);
            if (ok) return value;

            if (Builtins.BuiltinFunctions.TryGetValue(identifier.Value, out var fn))
            {
                return fn;
            }

            return new Error($"識別子が見つかりません。: {identifier.Value}");
        }

        public bool IsTruthly(IObject obj)
        {
            if (obj == Evaluator.True) return true;
            if (obj == Evaluator.False) return false;
            if (obj == Evaluator.Null) return false;
            return true;
        }

        public bool IsError(IObject obj)
        {
            if (obj != null) return obj.Type() == ObjectType.ERROR_OBJ;
            return false;
        }
    }
}
