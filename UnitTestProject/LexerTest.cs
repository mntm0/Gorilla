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
    }
}
