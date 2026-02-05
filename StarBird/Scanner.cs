namespace StarBird;

public class Scanner {
    private readonly String source;
    private readonly List<Token> tokens = new List<Token>();
    
    private int start;
    private int current;
    private int line = 1;
    
    private static readonly Dictionary<string, TokenType> keywords = new()
    {
        { "and",    TokenType.AND },
        { "class",  TokenType.CLASS },
        { "else",   TokenType.ELSE },
        { "false",  TokenType.FALSE },
        { "for",    TokenType.FOR },
        { "fun",    TokenType.FUN },
        { "if",     TokenType.IF },
        { "null",    TokenType.NULL },
        { "or",     TokenType.OR },
        { "print",  TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super",  TokenType.SUPER },
        { "this",   TokenType.THIS },
        { "true",   TokenType.TRUE },
        { "var",    TokenType.VAR },
        { "while",  TokenType.WHILE }
    };
    
    public Scanner(String source) {
        this.source = source;
    }
    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    public List<Token> ScanTokens() {
        while (!IsAtEnd()) {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }
    
    private void ScanToken() {
        char c = Advance();
        switch (c) {
            case '(': addToken(TokenType.LEFT_PAREN); break;
            case ')': addToken(TokenType.RIGHT_PAREN); break;
            case '{': addToken(TokenType.LEFT_BRACE); break;
            case '}': addToken(TokenType.RIGHT_BRACE); break;
            case ',': addToken(TokenType.COMMA); break;
            case '.': addToken(TokenType.DOT); break;
            case '-': addToken(TokenType.MINUS); break;
            case '+': addToken(TokenType.PLUS); break;
            case ';': addToken(TokenType.SEMICOLON); break;
            case '*': addToken(TokenType.STAR); break; 
            case '!':
                addToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                addToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                addToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                addToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/')) {
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                } else {
                    addToken(TokenType.SLASH);
                }
                break;
            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                line++;
                break;
            case '"': String(); break;
            case 'o':
                if (Match('r')) {
                    addToken(TokenType.OR);
                }
                else if (isAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    StarBird.Error(line, "Unexpected character.");
                }
                break;

            default:
                if (isDigit(c)) {
                    Number();
                } else if (isAlpha(c)) {
                    Identifier();
                } else {
                    StarBird.Error(line, "Unexpected character.");
                }
                break;

        }
    }
    private bool Match(char expected) 
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }
    
    private char Advance() 
    {
        return source[current++];
    }

    private void addToken(TokenType type) 
    {
        addToken(type, null);
    }

    private void addToken(TokenType type, Object literal) 
    {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }
    
    private void String() 
    {
        while (Peek() != '"' && !IsAtEnd()) {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd()) {
            StarBird.Error(line, "Unterminated string.");
            return;
        }
        
        Advance();
        
        string value = source.Substring(start + 1, current - start - 2);
        addToken(TokenType.STRING, value);
    }
    
    private bool isDigit(char c) {
        return c >= '0' && c <= '9';
    } 
    
    private void Number() {
        while (isDigit(Peek())) Advance();
        
        if (Peek() == '.' && isDigit(PeekNext())) {
            Advance();

            while (isDigit(Peek())) Advance();
        }

        string numberText = source.Substring(start, current - start);
        addToken(TokenType.NUMBER, Double.Parse(numberText, System.Globalization.CultureInfo.InvariantCulture));

    }
    
    private char PeekNext() {
        if (current + 1 >= source.Length - 1) return '\0';
        return source[current + 1];
    } 
    
    private char Peek() 
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }
    
    private void Identifier() {
        while (isAlphaNumeric(Peek())) Advance();
        string text = source.Substring(start, current - start);
        TokenType type;
        if (!keywords.TryGetValue(text, out type))
        {
            type = TokenType.IDENTIFIER;
        }

        addToken(type);
    }
    
    private bool isAlpha(char c) {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_';
    }

    private bool isAlphaNumeric(char c) {
        return isAlpha(c) || isDigit(c);
    }
}