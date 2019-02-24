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
            var input = @"let x = 5;
let y = 10;
let xyz = 838383;";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this._CheckParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 3,
                "Root.Statementsの数が間違っています。"
            );

            var tests = new string[] { "x", "y", "xyz" };
            for (int i = 0; i < tests.Length; i++)
            {
                var name = tests[i];
                var statement = root.Statements[i];
                this._TestLetStatement(statement, name);
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
            var input = @"return 5;
return 10;
return = 993322;";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this._CheckParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 3,
                "Root.Statementsの数が間違っています。"
            );

            foreach (var statement in root.Statements)
            {
                var returnStatement = statement as ReturnStatement;
                if (returnStatement == null)
                {
                    Assert.Fail("statement が ReturnStatement ではありません。");
                }

                Assert.AreEqual(
                    returnStatement.TokenLiteral(), "return",
                    $"return のリテラルが間違っています。"
                );
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
    }
}
