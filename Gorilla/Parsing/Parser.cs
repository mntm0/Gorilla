using Gorilla.Ast;
using Gorilla.Ast.Expressions;
using Gorilla.Ast.Statements;
using Gorilla.Lexing;
using System.Collections.Generic;

namespace Gorilla.Parsing
{
    public class Parser
    {
        public Token CurrentToken { get; set; }
        public Token NextToken { get; set; }
        public Lexer Lexer { get; }
        public List<string> Errors { get; set; } = new List<string>();

        public Parser(Lexer lexer)
        {
            this.Lexer = lexer;

            // 2つ分のトークンを読み込んでセットしておく
            this.CurrentToken = this.Lexer.NextToken();
            this.NextToken = this.Lexer.NextToken();
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
                default:
                    return null;
            }
        }

        public IExpression ParseExpression()
        {
            return null;
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
    }
}
