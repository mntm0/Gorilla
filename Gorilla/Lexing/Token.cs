using System.Collections.Generic;

namespace Gorilla.Lexing
{
    public class Token
    {
        public Token(TokenType type, string literal)
        {
            this.Type = type;
            this.Literal = literal;
        }
        public TokenType Type { get; set; }
        public string Literal { get; set; }

        public static TokenType LookupIdentifier(string identifier)
        {
            if (Token.Keywords.ContainsKey(identifier))
            {
                return Keywords[identifier];
            }
            return TokenType.IDENT;
        }

        public static Dictionary<string, TokenType> Keywords
            = new Dictionary<string, TokenType>() {
            { "let", TokenType.LET },
            { "fn", TokenType.FUNCTION },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "return", TokenType.RETURN },
            { "true", TokenType.TRUE },
            { "false", TokenType.FALSE },
        };
    }

    public enum TokenType
    {
        // 不正なトークン, 終端
        ILLEGAL,
        EOF,
        // 識別子、整数リテラル
        IDENT,
        INT,
        STRING,
        // 演算子
        ASSIGN,
        PLUS,
        MINUS,
        ASTERISK,
        SLASH,
        BANG,
        LT,
        GT,
        EQ,
        NOT_EQ,
        // デリミタ
        COMMA,
        SEMICOLON,
        // 括弧(){}
        LPAREN,
        RPAREN,
        LBRACE,
        RBRACE,
        // キーワード
        FUNCTION,
        LET,
        IF,
        ELSE,
        TRUE,
        FALSE,
        RETURN,
        // その他必要になったら追加します。
    }
}
