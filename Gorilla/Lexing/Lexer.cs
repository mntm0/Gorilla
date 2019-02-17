namespace Gorilla.Lexing
{
    public class Lexer
    {
        public string Input { get; private set; }
        public char CurrentChar { get; private set; }
        public char NextChar { get; private set; }
        public int Position { get; private set; } = 0;

        public Lexer(string input)
        {
            this.Input = input;
            this.ReadChar();
        }

        private void ReadChar()
        {
            if (this.Position >= this.Input.Length)
            {
                this.CurrentChar = (char)0;
            }
            else
            {
                this.CurrentChar = this.Input[this.Position];
            }

            if (this.Position + 1 >= this.Input.Length)
            {
                this.NextChar = (char)0;
            }
            else
            {
                this.NextChar = this.Input[this.Position + 1];
            }

            this.Position += 1;
        }

        public Token NextToken()
        {
            this.SkipWhiteSpace();
            Token token = null;
            switch (this.CurrentChar)
            {
                case '=':
                    if (this.NextChar == '=')
                    {
                        token = new Token(TokenType.EQ, "==");
                        this.ReadChar();
                    }
                    else
                    {
                        token = new Token(TokenType.ASSIGN, this.CurrentChar.ToString());
                    }
                    break;
                case '+':
                    token = new Token(TokenType.PLUS, this.CurrentChar.ToString());
                    break;
                case '-':
                    token = new Token(TokenType.MINUS, this.CurrentChar.ToString());
                    break;
                case '*':
                    token = new Token(TokenType.ASTERISK, this.CurrentChar.ToString());
                    break;
                case '/':
                    token = new Token(TokenType.SLASH, this.CurrentChar.ToString());
                    break;
                case '!':
                    if (this.NextChar == '=')
                    {
                        token = new Token(TokenType.NOT_EQ, "!=");
                        this.ReadChar();
                    }
                    else
                    {
                        token = new Token(TokenType.BANG, this.CurrentChar.ToString());
                    }
                    break;
                case '>':
                    token = new Token(TokenType.GT, this.CurrentChar.ToString());
                    break;
                case '<':
                    token = new Token(TokenType.LT, this.CurrentChar.ToString());
                    break;
                case ',':
                    token = new Token(TokenType.COMMA, this.CurrentChar.ToString());
                    break;
                case ';':
                    token = new Token(TokenType.SEMICOLON, this.CurrentChar.ToString());
                    break;
                case '(':
                    token = new Token(TokenType.LPAREN, this.CurrentChar.ToString());
                    break;
                case ')':
                    token = new Token(TokenType.RPAREN, this.CurrentChar.ToString());
                    break;
                case '{':
                    token = new Token(TokenType.LBRACE, this.CurrentChar.ToString());
                    break;
                case '}':
                    token = new Token(TokenType.RBRACE, this.CurrentChar.ToString());
                    break;
                case (char)0:
                    token = new Token(TokenType.EOF, "");
                    break;
                default:
                    if (this.IsLetter(this.CurrentChar))
                    {
                        var identifier = this.ReadIdentifier();
                        var type = Token.LookupIdentifier(identifier);
                        token = new Token(type, identifier);
                    }
                    else if (this.IsDigit(this.CurrentChar))
                    {
                        var number = this.ReadNumber();
                        token = new Token(TokenType.INT, number);
                    }
                    else
                    {
                        token = new Token(TokenType.ILLEGAL, this.CurrentChar.ToString());
                    }
                    break;
            }

            this.ReadChar();
            return token;
        }

        private string ReadIdentifier()
        {
            var identifier = this.CurrentChar.ToString();

            // 次の文字が Letter であればそれを読んで加える
            while (this.IsLetter(this.NextChar))
            {
                identifier += this.NextChar;
                this.ReadChar();
            }

            return identifier;
        }

        private bool IsLetter(char c)
        {
            return ('a' <= c && c <= 'z')
                || ('A' <= c && c <= 'Z')
                || c == '_';
        }

        private string ReadNumber()
        {
            var number = this.CurrentChar.ToString();

            // 次の文字が Digit であればそれを読んで加える
            while (this.IsDigit(this.NextChar))
            {
                number += this.NextChar;
                this.ReadChar();
            }

            return number;
        }

        private bool IsDigit(char c)
        {
            return '0' <= c && c <= '9';
        }

        private void SkipWhiteSpace()
        {
            while (this.CurrentChar == ' '
                || this.CurrentChar == '\t'
                || this.CurrentChar == '\r'
                || this.CurrentChar == '\n')
            {
                this.ReadChar();
            }
        }
    }
}
