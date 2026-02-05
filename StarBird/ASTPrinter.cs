using System.Text;

namespace StarBird;

public class ASTPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr) => expr.Accept(this);

    public string VisitBinaryExpr(Expr.Binary expr)
    {
        return Parenthesize(expr.op.Lexeme, expr.left, expr.right);
    }

    public string VisitGroupingExpr(Expr.Grouping expr)
    {
        return Parenthesize("group", expr.expression);
    }

    public string VisitLiteralExpr(Expr.Literal expr)
    {
        if (expr.value == null) return "nil";
        return expr.value.ToString();
    }

    public string VisitUnaryExpr(Expr.Unary expr)
    {
        return Parenthesize(expr.op.Lexeme, expr.right);
    }
    
    public string VisitAssignExpr(Expr.Assign expr)
    {
        return Parenthesize(expr.name.Lexeme, expr.value);
    }

    public string VisitLogicalExpr(Expr.Logical expr)
    {
        throw new NotImplementedException();
    }

    public string VisitVariableExpr(Expr.Variable expr)
    {
        return expr.name.Lexeme;
    }


    private string Parenthesize(string name, params Expr[] exprs)
    {
        var builder = new StringBuilder();

        builder.Append("(").Append(name);
        foreach (var expr in exprs)
        {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }

        builder.Append(")");

        return builder.ToString();
    }
    
    public static void main(String[] args)
    {
        Expr expression = new Expr.Binary(
            new Expr.Unary(
                new Token(TokenType.MINUS, "-", null, 1),
                new Expr.Literal(123)),
            new Token(TokenType.STAR, "*", null, 1),
            new Expr.Grouping(
                new Expr.Literal(45.67)));

        Console.WriteLine(new ASTPrinter().Print(expression));
    }
}