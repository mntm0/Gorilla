using Gorilla.Ast;
using Gorilla.Ast.Expressions;
using Gorilla.Ast.Statements;
using Gorilla.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTestProject
{
    [TestClass]
    public class AstTest
    {
        [TestMethod]
        public void TestNodeToCode1()
        {
            var code = "let x = abc;";

            var root = new Root();
            root.Statements = new List<IStatement>();

            root.Statements.Add(
                new LetStatement()
                {
                    Token = new Token(TokenType.LET, "let"),
                    Name = new Identifier(
                        new Token(TokenType.IDENT, "x"),
                        "x"
                    ),
                    Value = new Identifier(
                        new Token(TokenType.IDENT, "abc"),
                        "abc"
                    ),
                } 
            );

            Assert.AreEqual(code, root.ToCode(), "Root.ToCode() の結果が間違っています。");
        }
    }
}
