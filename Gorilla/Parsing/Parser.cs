using Gorilla.Ast;
using Gorilla.Ast.Expressions;
using Gorilla.Ast.Statements;
using Gorilla.Lexing;
using System;
using System.Collections.Generic;

namespace Gorilla.Parsing
{
    using PrefixParseFn = Func<IExpression>;
    using InfixParseFn = Func<IExpression, IExpression>;

    public class Parser
    {
        public Token CurrentToken { get; set; }
        public Token NextToken { get; set; }
        public Lexer Lexer { get; }
        public List<string> Errors { get; set; } = new List<string>();
        public Dictionary<TokenType, PrefixParseFn> PrefixParseFns { get; set; }
        public Dictionary<TokenType, InfixParseFn> InfixParseFns { get; set; }

        public Parser(Lexer lexer)
        {
            this.Lexer = lexer;

            // 2つ分のトークンを読み込んでセットしておく
            this.CurrentToken = this.Lexer.NextToken();
            this.NextToken = this.Lexer.NextToken();

            // トークンの種類と解析関数を関連付ける
            this.RegisterPrefixParseFns();
        }

        private void RegisterPrefixParseFns()
        {
            this.PrefixParseFns = new Dictionary<TokenType, PrefixParseFn>();
            this.PrefixParseFns.Add(TokenType.IDENT, this.ParseIdentifier);
            this.PrefixParseFns.Add(TokenType.INT, this.ParseIntegerLiteral);
            this.PrefixParseFns.Add(TokenType.BANG, this.ParsePrefixExpression);
            this.PrefixParseFns.Add(TokenType.MINUS, this.ParsePrefixExpression);
        }

        private void ReadToken()
        {
            this.CurrentToken = this.NextToken;
            this.NextToken = this.Lexer.NextToken();
        }

        public Root ParseProgram()
        {
            var root = new Root();
            root.Statements = new List<IStatement>();

            while (this.CurrentToken.Type != TokenType.EOF)
            {
                var statement = this.ParseStatement();
                if (statement != null) root.Statements.Add(statement);

                this.ReadToken();
            }
            return root;
        }

        public IStatement ParseStatement()
        {
            switch (this.CurrentToken.Type)
            {
                case TokenType.LET:
                    return this.ParseLetStatement();
                case TokenType.RETURN:
                    return this.ParseReturnStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        public IExpression ParseExpression(Precedence precedence)
        {
            this.PrefixParseFns.TryGetValue(this.CurrentToken.Type, out var prefix);
            if (prefix == null)
            {
                this.AddPrefixParseFnError(this.CurrentToken.Type);
                return null;
            }

            var leftExpression = prefix();
            return leftExpression;
        }

        public IExpression ParseIdentifier()
        {
            return new Identifier(this.CurrentToken, this.CurrentToken.Literal);
        }

        public IExpression ParseIntegerLiteral()
        {
            // リテラルを整数値に変換
            if (int.TryParse(this.CurrentToken.Literal, out int result))
            {
                return new IntegerLiteral()
                {
                    Token = this.CurrentToken,
                    Value = result,
                };
            }

            // 型変換失敗時
            var message = $"{this.CurrentToken.Literal} を integer に変換できません。";
            this.Errors.Add(message);
            return null;
        }

        public IExpression ParsePrefixExpression()
        {
            var expression = new PrefixExpression()
            {
                Token = this.CurrentToken,
                Operator = this.CurrentToken.Literal
            };

            this.ReadToken();

            expression.Right = this.ParseExpression(Precedence.PREFIX);
            return expression;
        }

        public LetStatement ParseLetStatement()
        {
            var statement = new LetStatement();
            statement.Token = this.CurrentToken;

            if (!this.ExpectPeek(TokenType.IDENT)) return null;

            // 識別子(let文の左辺)
            statement.Name = new Identifier(CurrentToken, this.CurrentToken.Literal);

            // 等号 =
            if (!this.ExpectPeek(TokenType.ASSIGN)) return null;

            // 式(let文の右辺)
            // TODO: 後で実装。
            while (this.CurrentToken.Type != TokenType.SEMICOLON)
            {
                // セミコロンが見つかるまで
                this.ReadToken();
            }

            return statement;
        }

        public ReturnStatement ParseReturnStatement()
        {
            var statement = new ReturnStatement();
            statement.Token = this.CurrentToken;
            this.ReadToken();

            // TODO: 後で実装。
            while (this.CurrentToken.Type != TokenType.SEMICOLON)
            {
                // セミコロンが見つかるまで
                this.ReadToken();
            }

            return statement;
        }

        public ExpressionStatement ParseExpressionStatement()
        {
            var statement = new ExpressionStatement();
            statement.Token = this.CurrentToken;

            statement.Expression = this.ParseExpression(Precedence.LOWEST);

            // セミコロンを読み飛ばす(省略可能)
            if (this.NextToken.Type == TokenType.SEMICOLON) this.ReadToken();

            return statement;
        }

        private bool ExpectPeek(TokenType type)
        {
            // 次のトークンが期待するものであれば読み進める
            if (this.NextToken.Type == type)
            {
                this.ReadToken();
                return true;
            }
            this.AddNextTokenError(type, this.NextToken.Type);
            return false;
        }

        private void AddNextTokenError(TokenType expected, TokenType actual)
        {
            this.Errors.Add($"{actual.ToString()} ではなく {expected.ToString()} が来なければなりません。");
        }

        private void AddPrefixParseFnError(TokenType tokenType)
        {
            var message = $"{tokenType.ToString()} に関連付けられた Prefix Parse Function が存在しません。";
            this.Errors.Add(message);
        }
    }

    public enum Precedence
    {
        LOWEST = 1,
        EQUALS,      // ==
        LESSGREATER, // >, <
        SUM,         // +
        PRODUCT,     // *
        PREFIX,      // -x, !x
        CALL,        // myFunction(x)
    }
}
