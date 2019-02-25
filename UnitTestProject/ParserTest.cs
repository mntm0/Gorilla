using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gorilla.Lexing;
using Gorilla.Parsing;
using Gorilla.Ast.Statements;
using Gorilla.Ast;
using Gorilla.Ast.Expressions;

namespace UnitTestProject
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void TestLetStatement1()
        {
            var tests = new(string, string, object)[]
            {
                ("let x = 5;", "x", 5),
                ("let y = true;", "y", true),
                ("let z = x;", "z", "x"),
            };

            foreach (var (input, name, expected) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                this._CheckParserErrors(parser);

                Assert.AreEqual(
                    root.Statements.Count, 1,
                    "Root.Statementsの数が間違っています。"
                );

                var statement = root.Statements[0];
                this._TestLetStatement(statement, name);

                var value = (statement as LetStatement).Value;
                this._TestLiteralExpression(value, expected);
            }
        }

        private void _TestLetStatement(IStatement statement, string name)
        {
            Assert.AreEqual(
                statement.TokenLiteral(), "let",
                "TokenLiteral が let ではありません。"
            );

            var letStatement = statement as LetStatement;
            if (letStatement == null)
            {
                Assert.Fail("statement が LetStatement ではありません。");
            }

            this._TestIdentifier(letStatement.Name, name);
        }

        [TestMethod]
        public void TestReturnStatement1()
        {
            var tests = new(string, object)[]
            {
                ("return 5;", 5),
                ("return true;", true),
                ("return x;", "x"),
            };

            foreach (var (input, expected) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                this._CheckParserErrors(parser);

                Assert.AreEqual(
                    root.Statements.Count, 1,
                    "Root.Statementsの数が間違っています。"
                );

                var returnStatement = root.Statements[0] as ReturnStatement;
                if (returnStatement == null)
                {
                    Assert.Fail("statement が ReturnStatement ではありません。");
                }

                Assert.AreEqual(
                    returnStatement.TokenLiteral(), "return",
                    $"return のリテラルが間違っています。"
                );

                this._TestLiteralExpression(returnStatement.ReturnValue, expected);
            }
        }

        private void _CheckParserErrors(Parser parser)
        {
            if (parser.Errors.Count == 0) return;
            var message = "\n" + string.Join("\n", parser.Errors);
            Assert.Fail(message);
        }

        [TestMethod]
        public void TestIdentifierExpression1()
        {
            var input = @"foobar;";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this._CheckParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 1,
                "Root.Statementsの数が間違っています。"
            );

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statement が ExpressionStatement ではありません。");
            }

            this._TestIdentifier(statement.Expression, "foobar");
        }

        [TestMethod]
        public void TestIntegerLiteralExpression1()
        {
            var input = @"123;";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this._CheckParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 1,
                "Root.Statementsの数が間違っています。"
            );

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statement が ExpressionStatement ではありません。");
            }

            this._TestIntegerLiteral(statement.Expression, 123);
        }

        [TestMethod]
        public void TestPrefixExpressions1()
        {
            var tests = new[] {
                ("!5", "!", 5),
                ("-15", "-", 15),
            };

            foreach (var (input, op, value) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                this._CheckParserErrors(parser);

                Assert.AreEqual(
                    root.Statements.Count, 1,
                    "Root.Statementsの数が間違っています。"
                );

                var statement = root.Statements[0] as ExpressionStatement;
                if (statement == null)
                {
                    Assert.Fail("statement が ExpressionStatement ではありません。");
                }

                var expression = statement.Expression as PrefixExpression;
                if (expression == null)
                {
                    Assert.Fail("expression が PrefixExpression ではありません。");
                }

                if (expression.Operator != op)
                {
                    Assert.Fail($"Operator が {expression.Operator} ではありません。({op})");
                }

                this._TestIntegerLiteral(expression.Right, value);
            }
        }

        public void _TestIntegerLiteral(IExpression expression, int value)
        {
            var integerLiteral = expression as IntegerLiteral;
            if (integerLiteral == null)
            {
                Assert.Fail("Expression が IntegerLiteral ではありません。");
            }
            if (integerLiteral.Value != value)
            {
                Assert.Fail($"integerLiteral.Value が {value} ではありません。");
            }
            if (integerLiteral.TokenLiteral() != $"{value}")
            {
                Assert.Fail($"ident.TokenLiteral が {value} ではありません。");
            }
        }

        [TestMethod]
        public void TestInfixExpressions1()
        {
            var tests = new (string, object, string, object)[] {
                ("1 + 1;", 1, "+", 1),
                ("1 - 1;", 1, "-", 1),
                ("1 * 1;", 1, "*", 1),
                ("1 / 1;", 1, "/", 1),
                ("1 < 1;", 1, "<", 1),
                ("1 > 1;", 1, ">", 1),
                ("1 == 1;", 1, "==", 1),
                ("1 != 1;", 1, "!=", 1),
                ("true == false", true, "==", false),
                ("false != false", false, "!=", false),
            };

            foreach (var (input, leftValue, op, rightValue) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                this._CheckParserErrors(parser);

                Assert.AreEqual(
                    root.Statements.Count, 1,
                    "Root.Statementsの数が間違っています。"
                );

                var statement = root.Statements[0] as ExpressionStatement;
                if (statement == null)
                {
                    Assert.Fail("statement が ExpressionStatement ではありません。");
                }

                this._TestInfixExpression(statement.Expression, leftValue, op, rightValue);
            }
        }

        [TestMethod]
        public void TestOperatorPrecedenceParsing()
        {
            var tests = new[]
            {
                ("a + b", "(a + b)"),
                ("!-a", "(!(-a))"),
                ("a + b - c", "((a + b) - c)"),
                ("a * b / c", "((a * b) / c)"),
                ("a + b * c", "(a + (b * c))"),
                ("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)"),
                ("1 + 2; -3 * 4", "(1 + 2)\r\n((-3) * 4)"),
                ("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))"),
                ("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
                ("true", "true"),
                ("true == false", "(true == false)"),
                ("1 > 2 == false", "((1 > 2) == false)"),
                ("(1 + 2) * 3", "((1 + 2) * 3)"),
                ("1 + (2 - 3)", "(1 + (2 - 3))"),
                ("-(1 + 2)", "(-(1 + 2))"),
                ("!(true == true)", "(!(true == true))"),
                ("1 + (2 - 3) * 4", "(1 + ((2 - 3) * 4))"),
                ("(1 + -(2 + 3)) * 4", "((1 + (-(2 + 3))) * 4)"),
                ("add(1, 2) + 3 > 4", "((add(1, 2) + 3) > 4)"),
                ("add(x, y, 1, 2*3, 4+5, add(z) )", "add(x, y, 1, (2 * 3), (4 + 5), add(z))"),
                ("add(1 + 2 - 3 * 4 / 5 + 6)", "add((((1 + 2) - ((3 * 4) / 5)) + 6))"),
            };

            foreach (var (input, code) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                this._CheckParserErrors(parser);

                var actual = root.ToCode();
                Assert.AreEqual(code, actual);
            }
        }

        private void _TestIdentifier(IExpression expression, string value)
        {
            var ident = expression as Identifier;
            if (ident == null)
            {
                Assert.Fail("Expression が Identifier ではありません。");
            }
            if (ident.Value != value)
            {
                Assert.Fail($"ident.Value が {value} ではありません。({ident.Value})");
            }
            if (ident.TokenLiteral() != value)
            {
                Assert.Fail($"ident.TokenLiteral が {value} ではありません。({ident.TokenLiteral()})");
            }
        }

        private void _TestLiteralExpression(IExpression expression, object expected)
        {
            switch (expected)
            {
                case int intValue:
                    this._TestIntegerLiteral(expression, intValue);
                    break;
                case string stringValue:
                    this._TestIdentifier(expression, stringValue);
                    break;
                case bool boolValue:
                    this._TestBooleanLiteral(expression, boolValue);
                    break;
                default:
                    Assert.Fail("予期せぬ型です。");
                    break;
            }
        }

        private void _TestInfixExpression(IExpression expression, object left, string op, object right)
        {
            var infixExpression = expression as InfixExpression;
            if (infixExpression == null)
            {
                Assert.Fail("expression が InfixExpression ではありません。");
            }

            this._TestLiteralExpression(infixExpression.Left, left);

            if (infixExpression.Operator != op)
            {
                Assert.Fail($"Operator が {infixExpression.Operator} ではありません。({op})");
            }

            this._TestLiteralExpression(infixExpression.Right, right);
        }

        [TestMethod]
        public void TestBooleanLiteralExpression()
        {
            var tests = new[]
            {
                ("true;", true),
                ("false;", false),
            };

            foreach (var (input, value) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                this._CheckParserErrors(parser);

                Assert.AreEqual(
                    root.Statements.Count, 1,
                    "Root.Statementsの数が間違っています。"
                );

                var statement = root.Statements[0] as ExpressionStatement;
                if (statement == null)
                {
                    Assert.Fail("statement が ExpressionStatement ではありません。");
                }

                this._TestBooleanLiteral(statement.Expression, value);
            }
        }

        private void _TestBooleanLiteral(IExpression expression, bool value)
        {
            var booleanLiteral = expression as BooleanLiteral;
            if (booleanLiteral == null)
            {
                Assert.Fail("Expression が BooleanLiteral ではありません。");
            }
            if (booleanLiteral.Value != value)
            {
                Assert.Fail($"booleanLiteral.Value が {value} ではありません。({booleanLiteral.Value})");
            }
            if (booleanLiteral.TokenLiteral() != value.ToString().ToLower())
            {
                Assert.Fail($"booleanLiteral.TokenLiteral が {value.ToString().ToLower()} ではありません。({booleanLiteral.TokenLiteral()})");
            }
        }

        [TestMethod]
        public void TestIfExpression()
        {
            var input = "if (x < y) { x }";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this._CheckParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 1,
                "Root.Statementsの数が間違っています。"
            );

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statement が ExpressionStatement ではありません。");
            }

            var expression = statement.Expression as IfExpression;
            if (expression == null)
            {
                Assert.Fail("expression が IfExpression ではありません。");
            }

            this._TestInfixExpression(expression.Condition, "x", "<", "y");

            if (expression.Consequence.Statements.Count != 1)
            {
                Assert.Fail("Consequence の 文の数が 1 ではありません。");
            }

            var consequence = expression.Consequence.Statements[0] as ExpressionStatement;
            if (consequence == null)
            {
                Assert.Fail("consequence が ExpressionStatement ではありません。");
            }

            this._TestIdentifier(consequence.Expression, "x");

            if (expression.Alternative != null)
            {
                Assert.Fail("expression.Alternative が null ではありません。");
            }
        }

        [TestMethod]
        public void TestIfElseExpression()
        {
            var input = "if (x < y) { x } else { y; }";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this._CheckParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 1,
                "Root.Statementsの数が間違っています。"
            );

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statement が ExpressionStatement ではありません。");
            }

            var expression = statement.Expression as IfExpression;
            if (expression == null)
            {
                Assert.Fail("expression が IfExpression ではありません。");
            }

            this._TestInfixExpression(expression.Condition, "x", "<", "y");

            if (expression.Consequence.Statements.Count != 1)
            {
                Assert.Fail("Consequence の 文の数が 1 ではありません。");
            }

            var consequence = expression.Consequence.Statements[0] as ExpressionStatement;
            if (consequence == null)
            {
                Assert.Fail("consequence が ExpressionStatement ではありません。");
            }
            this._TestIdentifier(consequence.Expression, "x");

            if (expression.Alternative == null)
            {
                Assert.Fail("expression.Alternative が null です。");
            }

            if (expression.Alternative.Statements.Count != 1)
            {
                Assert.Fail("Consequence の 文の数が 1 ではありません。");
            }

            var alternative = expression.Alternative.Statements[0] as ExpressionStatement;
            if (consequence == null)
            {
                Assert.Fail("alternative が ExpressionStatement ではありません。");
            }
            this._TestIdentifier(alternative.Expression, "y");
        }

        [TestMethod]
        public void TestFunctionLiteral()
        {
            var input = "fn(x, y) { x + y; }";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this._CheckParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 1,
                "Root.Statementsの数が間違っています。"
            );

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statement が ExpressionStatement ではありません。");
            }

            var expression = statement.Expression as FunctionLiteral;
            if (expression == null)
            {
                Assert.Fail("expression が FunctionLiteral ではありません。");
            }

            Assert.AreEqual(
                expression.Parameters.Count, 2,
                "関数リテラルの引数の数が間違っています。"
            );
            this._TestIdentifier(expression.Parameters[0], "x");
            this._TestIdentifier(expression.Parameters[1], "y");

            Assert.AreEqual(
                expression.Body.Statements.Count, 1,
                "関数リテラルの本文の式の数が間違っています。"
            );

            var bodyStatement = expression.Body.Statements[0] as ExpressionStatement;
            if (bodyStatement == null)
            {
                Assert.Fail("bodyStatement が ExpressionStatement ではありません。");
            }
            this._TestInfixExpression(bodyStatement.Expression, "x", "+", "y");
        }

        [TestMethod]
        public void TestFunctionParameter()
        {
            var tests = new[]
            {
                ("fn() {};", new string[] { }),
                ("fn(x) {};", new string[] { "x" }),
                ("fn(x, y, z) {};", new string[] { "x", "y", "z" }),
            };

            foreach (var (input, parameters) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();

                var statement = root.Statements[0] as ExpressionStatement;
                var fn = statement.Expression as FunctionLiteral;
                
                Assert.AreEqual(
                    fn.Parameters.Count, parameters.Length,
                    "関数リテラルの引数の数が間違っています。"
                );
                for (int i = 0; i < parameters.Length; i++)
                {
                    this._TestIdentifier(fn.Parameters[i], parameters[i]);
                }
            }
        }

        [TestMethod]
        public void TestCallExpression()
        {
            var input = "add(1, 2 * 3, 4 + 5);";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this._CheckParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 1,
                "Root.Statementsの数が間違っています。"
            );

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statement が ExpressionStatement ではありません。");
            }

            var expression = statement.Expression as CallExpression;
            if (expression == null)
            {
                Assert.Fail("expression が CallExpression ではありません。");
            }

            this._TestIdentifier(expression.Function, "add");

            Assert.AreEqual(
                expression.Arguments.Count, 3,
                "関数リテラルの引数の数が間違っています。"
            );

            this._TestLiteralExpression(expression.Arguments[0], 1);
            this._TestInfixExpression(expression.Arguments[1], 2, "*", 3);
            this._TestInfixExpression(expression.Arguments[2], 4, "+", 5);
        }
    }
}
