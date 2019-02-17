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
            Token token = null;
            switch (this.CurrentChar)
            {
                case '=':
                    token = new Token(TokenType.ASSIGN, this.CurrentChar.ToString());
                    break;
                case '+':
                    token = new Token(TokenType.PLUS, this.CurrentChar.ToString());
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
            }

            this.ReadChar();
            return token;
        }
    }
}
