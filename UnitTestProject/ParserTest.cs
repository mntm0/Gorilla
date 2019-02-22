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

            Assert.AreEqual(
                letStatement.Name.Value, name,
                $"識別子が間違っています。"
            );

            Assert.AreEqual(
                letStatement.Name.TokenLiteral(), name,
                $"識別子のリテラルが間違っています。"
            );
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

            var ident = statement.Expression as Identifier;
            if (ident == null)
            {
                Assert.Fail("Expression が Identifier ではありません。");
            }
            if (ident.Value != "foobar")
            {
                Assert.Fail("ident.Value が foobar ではありません。");
            }
            if (ident.TokenLiteral() != "foobar")
            {
                Assert.Fail("ident.TokenLiteral が foobar ではありません。");
            }
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

            var integerLiteral = statement.Expression as IntegerLiteral;
            if (integerLiteral == null)
            {
                Assert.Fail("Expression が IntegerLiteral ではありません。");
            }
            if (integerLiteral.Value != 123)
            {
                Assert.Fail("integerLiteral.Value が 123 ではありません。");
            }
            if (integerLiteral.TokenLiteral() != "123")
            {
                Assert.Fail("integerLiteral.TokenLiteral が 123 ではありません。");
            }
        }
    }
}
