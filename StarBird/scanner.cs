namespace StarBird;

public class Scanner {
    private readonly String source;
    private readonly List<Token> tokens = new();
    
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
        { "nil",    TokenType.NIL },
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

    public List<Token> scanTokens() {
        while (!IsAtEnd()) {
            start = current;
            scanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }
    
    private void scanToken() {
        char c = advance();
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
                addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (match('/')) {
                    while (peek() != '\n' && !IsAtEnd()) advance();
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
                if (match('r')) {
                    addToken(TokenType.OR);
                }
                else if (isAlpha(c))
                {
                    identifier();
                }
                else
                {
                    StarBird.error(line, "Unexpected character.");
                }
                break;

            default:
                if (isDigit(c)) {
                    number();
                } else {
                    StarBird.error(line, "Unexpected character.");
                }
                break;
        }
    }
    private bool match(char expected) 
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }
    
    private char advance() 
    {
        return source[current++];
    }

    private void addToken(TokenType type) 
    {
        addToken(type, null);
    }

    private void addToken(TokenType type, Object literal) 
    {
        String text = source.Substring(start, current);
        tokens.Add(new Token(type, text, literal, line));
    }
    
    private void String() 
    {
        while (peek() != '"' && !IsAtEnd()) {
            if (peek() == '\n') line++;
            advance();
        }

        if (IsAtEnd()) {
            StarBird.error(line, "Unterminated string.");
            return;
        }
        
        advance();
        
        String value = source.Substring(start + 1, current - 1);
        addToken(TokenType.STRING, value);
    }
    
    private bool isDigit(char c) {
        return c >= '0' && c <= '9';
    } 
    
    private void number() {
        while (isDigit(peek())) advance();
        
        if (peek() == '.' && isDigit(peekNext())) {
            advance();

            while (isDigit(peek())) advance();
        }

        addToken(TokenType.NUMBER,
            Double.Parse(source.Substring(start, current)));
    }
    
    private char peekNext() {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    } 
    
    private char peek() 
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }
    
    private void identifier() {
        while (isAlphaNumeric(peek())) advance();
        String text = source.Substring(start, current);
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