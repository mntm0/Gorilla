using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Gorilla.Lexing;

namespace UnitTestProject
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void TestNextToken1()
        {
            var input = "=+(){},;";

            var testTokens = new List<Token>();
            testTokens.Add(new Token(TokenType.ASSIGN, "="));
            testTokens.Add(new Token(TokenType.PLUS, "+"));
            testTokens.Add(new Token(TokenType.LPAREN, "("));
            testTokens.Add(new Token(TokenType.RPAREN, ")"));
            testTokens.Add(new Token(TokenType.LBRACE, "{"));
            testTokens.Add(new Token(TokenType.RBRACE, "}"));
            testTokens.Add(new Token(TokenType.COMMA, ","));
            testTokens.Add(new Token(TokenType.SEMICOLON, ";"));
            testTokens.Add(new Token(TokenType.EOF, ""));

            var lexer = new Lexer(input);

            foreach (var testToken in testTokens)
            {
                var token = lexer.NextToken();
                Assert.AreEqual(testToken.Type, token.Type, "トークンの種類が間違っています。");
                Assert.AreEqual(testToken.Literal, token.Literal, "トークンのリテラルが間違っています。");
            }
        }

        [TestMethod]
        public void TestNextToken2()
        {
            var input = @"let five = 5;
let ten = 10;

let add = fn(x, y) {
    x + y;
};

let result = add(five, ten);";

            var testTokens = new List<Token>();
            // let five = 5;
            testTokens.Add(new Token(TokenType.LET, "let"));
            testTokens.Add(new Token(TokenType.IDENT, "five"));
            testTokens.Add(new Token(TokenType.ASSIGN, "="));
            testTokens.Add(new Token(TokenType.INT, "5"));
            testTokens.Add(new Token(TokenType.SEMICOLON, ";"));
            // let ten = 5;
            testTokens.Add(new Token(TokenType.LET, "let"));
            testTokens.Add(new Token(TokenType.IDENT, "ten"));
            testTokens.Add(new Token(TokenType.ASSIGN, "="));
            testTokens.Add(new Token(TokenType.INT, "10"));
            testTokens.Add(new Token(TokenType.SEMICOLON, ";"));
            // let add = fn(x, y) { x + y; };
            testTokens.Add(new Token(TokenType.LET, "let"));
            testTokens.Add(new Token(TokenType.IDENT, "add"));
            testTokens.Add(new Token(TokenType.ASSIGN, "="));
            testTokens.Add(new Token(TokenType.FUNCTION, "fn"));
            testTokens.Add(new Token(TokenType.LPAREN, "("));
            testTokens.Add(new Token(TokenType.IDENT, "x"));
            testTokens.Add(new Token(TokenType.COMMA, ","));
            testTokens.Add(new Token(TokenType.IDENT, "y"));
            testTokens.Add(new Token(TokenType.RPAREN, ")"));
            testTokens.Add(new Token(TokenType.LBRACE, "{"));
            testTokens.Add(new Token(TokenType.IDENT, "x"));
            testTokens.Add(new Token(TokenType.PLUS, "+"));
            testTokens.Add(new Token(TokenType.IDENT, "y"));
            testTokens.Add(new Token(TokenType.SEMICOLON, ";"));
            testTokens.Add(new Token(TokenType.RBRACE, "}"));
            testTokens.Add(new Token(TokenType.SEMICOLON, ";"));
            // let result = add(five, ten);
            testTokens.Add(new Token(TokenType.LET, "let"));
            testTokens.Add(new Token(TokenType.IDENT, "result"));
            testTokens.Add(new Token(TokenType.ASSIGN, "="));
            testTokens.Add(new Token(TokenType.IDENT, "add"));
            testTokens.Add(new Token(TokenType.LPAREN, "("));
            testTokens.Add(new Token(TokenType.IDENT, "five"));
            testTokens.Add(new Token(TokenType.COMMA, ","));
            testTokens.Add(new Token(TokenType.IDENT, "ten"));
            testTokens.Add(new Token(TokenType.RPAREN, ")"));
            testTokens.Add(new Token(TokenType.SEMICOLON, ";"));
            testTokens.Add(new Token(TokenType.EOF, ""));

            var lexer = new Lexer(input);

            foreach (var testToken in testTokens)
            {
                var token = lexer.NextToken();
                Assert.AreEqual(testToken.Type, token.Type, "トークンの種類が間違っています。");
                Assert.AreEqual(testToken.Literal, token.Literal, "トークンのリテラルが間違っています。");
            }
        }
    }
}
