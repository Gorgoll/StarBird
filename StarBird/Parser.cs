using Microsoft.VisualBasic.CompilerServices;

namespace StarBird;

public class Parser
{
    private class ParseError : Exception { }
    private List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public List<Stmt> Parse() {
        List<Stmt> statements = new();
        while (!IsAtEnd()) {
            statements.Add(Declaration());
        }

        return statements; 
    }

    private Expr Expression()
    {
        return Assignment();
    }

    private Stmt Declaration()
    {
        try
        {
            if (Match(TokenType.VAR)) return VarDeclaration();
            
            return Statement();
        }
        catch (ParseError e)
        {
            Synchronize();
            return null;
        }
        
    }
    
    private Stmt Statement()
    {
        if (Match(TokenType.IF)) return IfStatement();
        if (Match(TokenType.PRINT)) return PrintStatement();
        if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());
        
        return ExpressionStatement();
    }

    private Stmt IfStatement()
    {
          Consume(TokenType.LEFT_PAREN, "Expect '('.");
          Expr condition = Expression();
          Consume(TokenType.RIGHT_PAREN, "Expect ')'.");
          Stmt thenBranch = Statement();
          Stmt elseBranch = null;
          if (Match(TokenType.ELSE))
          {
              elseBranch = Statement();
          }

          return new Stmt.If(condition, thenBranch, elseBranch);
    }
    private Stmt PrintStatement() {
        Expr value = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new Stmt.Print(value);
    }

    private Stmt VarDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER,"Expect variable name.");

        Expr initializer = null;
        if (Match(TokenType.EQUAL))
        {
            initializer = Expression();
        }
        
        Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
        return new Stmt.Var(name, initializer);
    }
    
    private Stmt ExpressionStatement() {
        Expr expr = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new Stmt.Expression(expr);
    }

    private List<Stmt> Block()
    {
        List<Stmt> statements = [];

        while (!Check(TokenType.RIGHT_BRACE) &&  !IsAtEnd())
        {
            statements.Add(Declaration());
        }
        
        Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
        return statements;
    }

    private Expr Assignment()
    {
        Expr expr = Or();

        if (Match(TokenType.EQUAL))
        {
            Token equals = Previous();
            Expr value = Assignment();

            if (expr is Expr.Variable)
            {
                Token name = ((Expr.Variable)expr).name;
                return new Expr.Assign(name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Expr Or()
    {
        Expr expr = And();

        while (Match(TokenType.OR))
        {
            Token op = Previous();
            Expr right = And();
            expr = new Expr.Logical(expr, op, right);
        }

        return expr;
    }

    private Expr And()
    {
        Expr expr = Equality();

        while (Match(TokenType.AND))
        {
            Token op = Previous();
            Expr right = Equality();
            expr = new Expr.Logical(expr, op, right);
        }

        return expr;
    }

    private Expr Equality()
    {
        Expr expr= Comparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            Token op= Previous();
            Expr right = Comparison();
            expr = new Expr.Binary(expr, op, right);
        }
        
        return expr;
    }

    private bool Match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }


    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek()
    {
        return tokens[current];
    }

    private Token Previous()
    {
        return tokens[current-1];
    }

    private ParseError Error(Token token, string msg)
    { 
        ReportError(token, msg);
        return new ParseError();
    }


    private Expr Comparison()
    {
        Expr expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, 
                   TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op = Previous();
            Expr right = Term();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    private Expr Term()
    {
        Expr expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op = Previous();
            Expr right = Factor();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    private Expr Factor()
    {
        Expr expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            Token op = Previous();
            Expr right = Unary();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            Token op = Previous();
            Expr right = Unary();
            return new Expr.Unary(op, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE)) return new Expr.Literal(false);
        if (Match(TokenType.TRUE)) return new Expr.Literal(true);
        if (Match(TokenType.NULL)) return new Expr.Literal(null);

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Expr.Literal(Previous().Literal);
        }

        if (Match(TokenType.IDENTIFIER))
        {
            return new Expr.Variable(Previous());
        }
        
        if (Match(TokenType.LEFT_PAREN))
        {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_PAREN,"Expect ')' after expression");
            return new Expr.Grouping(expr);
        }
        throw Error(Peek(), "Expect expression.");
    }

    private Token Consume(TokenType type, string msg)
    {
        if (Check(type)) return Advance();
        
        throw Error(Peek() , msg);
    }

    static void ReportError(Token token, string msg)
    {
        if (token.Type == TokenType.EOF)
            Console.WriteLine();
        else
            Console.WriteLine();
    }


    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if(Previous().Type == TokenType.SEMICOLON) return;

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }
}