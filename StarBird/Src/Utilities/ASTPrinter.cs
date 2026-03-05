using System.Text;

namespace StarBird;

public class ASTPrinter :
    Expr.IVisitor<string>,
    Stmt.IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string Print(List<Stmt> statements)
    {
        var builder = new StringBuilder();

        foreach (var stmt in statements)
        {
            builder.AppendLine(stmt.Accept(this));
        }

        return builder.ToString();
    }

    public string VisitBinaryExpr(Expr.Binary expr)
        => Parenthesize(expr.op.Lexeme, expr.left, expr.right);

    public string VisitCallExpr(Expr.Call expr)
    {
        var builder = new StringBuilder();

        builder.Append("(call ")
               .Append(expr.callee.Accept(this));

        foreach (var arg in expr.arguments)
        {
            builder.Append(" ")
                   .Append(arg.Accept(this));
        }

        builder.Append(")");

        return builder.ToString();
    }

    public string VisitGroupingExpr(Expr.Grouping expr)
        => Parenthesize("group", expr.expression);

    public string VisitLiteralExpr(Expr.Literal expr)
        => expr.value == null ? "nil" : expr.value.ToString();

    public string VisitUnaryExpr(Expr.Unary expr)
        => Parenthesize(expr.op.Lexeme, expr.right);

    public string VisitAssignExpr(Expr.Assign expr)
        => Parenthesize("assign " + expr.name.Lexeme, expr.value);

    public string VisitLogicalExpr(Expr.Logical expr)
        => Parenthesize(expr.op.Lexeme, expr.left, expr.right);

    public string VisitVariableExpr(Expr.Variable expr)
        => expr.name.Lexeme;

    public string VisitExpressionStmt(Stmt.Expression stmt)
        => Parenthesize("expr", stmt.expression);

    public string VisitPrintStmt(Stmt.Print stmt)
        => Parenthesize("print", stmt.expression);

    public string VisitVarStmt(Stmt.Var stmt)
    {
        if (stmt.initializer == null)
            return "(var " + stmt.name.Lexeme + ")";

        return Parenthesize("var " + stmt.name.Lexeme, stmt.initializer);
    }

    public string VisitBlockStmt(Stmt.Block stmt)
    {
        var builder = new StringBuilder();
        builder.Append("(block");

        foreach (var s in stmt.statements)
        {
            builder.Append("\n  ")
                   .Append(s.Accept(this));
        }

        builder.Append(")");

        return builder.ToString();
    }

    public string VisitIfStmt(Stmt.If stmt)
        => Parenthesize("if", stmt.condition, stmt.thenBranch, stmt.elseBranch);

    public string VisitWhileStmt(Stmt.While stmt)
        => Parenthesize("while", stmt.condition, stmt.body);

    public string VisitFunctionStmt(Stmt.Function stmt)
    {
        var builder = new StringBuilder();

        builder.Append("(fun ")
               .Append(stmt.name.Lexeme)
               .Append(" (");

        for (int i = 0; i < stmt.param.Count; i++)
        {
            if (i > 0) builder.Append(" ");
            builder.Append(stmt.param[i].Lexeme);
        }

        builder.Append(") (block");

        foreach (var s in stmt.body)
        {
            builder.Append("\n  ")
                   .Append(s.Accept(this));
        }

        builder.Append("))");

        return builder.ToString();
    }

    public string VisitReturnStmt(Stmt.Return stmt)
        => Parenthesize("return", stmt.value);
    

    private string Parenthesize(string name, params object[] nodes)
    {
        var builder = new StringBuilder();

        builder.Append("(").Append(name);

        foreach (var node in nodes)
        {
            if (node == null)
                continue;

            if (node is Expr e)
                builder.Append(" ").Append(e.Accept(this));

            else if (node is Stmt s)
                builder.Append(" ").Append(s.Accept(this));
        }

        builder.Append(")");

        return builder.ToString();
    }
}