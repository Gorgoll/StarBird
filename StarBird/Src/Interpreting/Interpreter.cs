using System;
using System.Collections.Generic;

namespace StarBird;

public class Interpreter : Expr.IVisitor<object>,Stmt.IVisitor<object>
{
    public readonly Environment globals = new();
    private Environment environment;
    public Interpreter()
    {
        environment = globals;
        int arity()
        {
            return 0;
        }

        object call(Interpreter interpreter, List<object> arguments)
        {
            return (double)DateTime.Now.Millisecond / 1000;
        }

        string toString()
        {
            return "<native fn>";
        }
    }
    public void Interpret(List<Stmt> statements) { 
        try {
            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        } catch (RuntimeError error) {
            Console.Error.WriteLine(error);
        }
    }

    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.value;
    }

    public object VisitLogicalExpr(Expr.Logical expr)
    {
        object left = Evaluate(expr.left);

        if (expr.op.Type == TokenType.OR)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!IsTruthy(left)) return left;
        }

        return Evaluate(expr.right);
    }
    
    public object VisitGroupingExpr(Expr.Grouping expr) {
        return Evaluate(expr.expression);
    }
    
    private object Evaluate(Expr expr) {
        return expr.Accept(this);
    }

    private void Execute(Stmt stmt)
    {
            stmt.Accept(this);
    }

    public void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        Environment previous = this.environment;

        try
        {
            this.environment = environment;

            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            this.environment = previous;
        }
    }

    public object VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.statements, new Environment(environment));
        return null;
    }
    
    public object VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.expression);
        return null;
    }

    public object VisitFunctionStmt(Stmt.Function stmt)
    {
        StarBirdFunction function = new StarBirdFunction(stmt, environment);
        environment.Define(stmt.name.Lexeme, function);
        return null;
    }

    public object VisitIfStmt(Stmt.If stmt)
    {
        if (IsTruthy(Evaluate(stmt.condition)))
        {
            Execute(stmt.thenBranch);
        }
        else if (stmt.elseBranch != null)
        {
            Execute(stmt.elseBranch);
        }
        return null;
    }
    
    public object VisitPrintStmt(Stmt.Print stmt)
    {
        object value = Evaluate(stmt.expression);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object VisitReturnStmt(Stmt.Return stmt)
    {
        object value = null;
        if (stmt.value != null) value = Evaluate(stmt.value);

        throw new Return(value);
    }

    public object VisitVarStmt(Stmt.Var stmt)
    {
        object value = null;
        if (stmt.initializer != null)
        {
            value = Evaluate(stmt.initializer);
        }

        environment.Define(stmt.name.Lexeme, value);
        return null;
    }

    public object VisitWhileStmt(Stmt.While stmt)
    {
        while (IsTruthy(Evaluate(stmt.condition)))
        {
            Execute(stmt.body);
        }

        return null;
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        var value = Evaluate(expr.value);
        environment.Assign(expr.name, value);
        return value;
    }

    public object VisitVariableExpr(Expr.Variable expr)
    {
        return environment.Get(expr.name);
    }
    
    public object VisitBinaryExpr(Expr.Binary expr)
    {
        object left = Evaluate(expr.left);
        object right = Evaluate(expr.right);
        
        switch (expr.op.Type)
        {
            case TokenType.BANG_EQUAL: 
                return !IsEqual(left, right);
            
            case TokenType.EQUAL_EQUAL: 
                return IsEqual(left, right);
            
            case TokenType.GREATER:
                CheckNumberOperands(expr.op, left, right);
                return (double)left > (double)right;
            
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.op, left, right);
                return (double)left >= (double)right;
            
            case TokenType.LESS:
                CheckNumberOperands(expr.op, left, right);
                return (double)left < (double)right;
            
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.op, left, right);
                return (double)left <= (double)right;
            
            case TokenType.MINUS:
                CheckNumberOperands(expr.op, left, right);
                return (double)left - (double)right;
            
            case TokenType.PLUS:
                if (left is Double && right is Double)
                {
                    return (double)left + (double)right;
                }

                if (left is String && right is String)
                {
                    return (string)left + (string)right;
                }
                
                throw new RuntimeError(expr.op,
                    "Operands must be two numbers or two strings.");
            
            case TokenType.SLASH:
                CheckNumberOperands(expr.op, left, right);
                return (double)left / (double)right;
            
            case TokenType.STAR:
                CheckNumberOperands(expr.op, left, right);
                return (double)left * (double)right;
        }
        
        return null;
    }

    public object VisitCallExpr(Expr.Call expr)
    {
        object callee = Evaluate(expr.callee);

        List<object> args = [];
        foreach (var argument in expr.arguments)
        {
            args.Add(Evaluate(argument));
        }

        if (!(callee is StarBirdCallable)) {
            throw new RuntimeError(expr.paren,
                "Can only call functions and classes.");
        }

        StarBirdCallable function = (StarBirdCallable)callee;
        if (args.Count != function.arity()) {
            throw new RuntimeError(expr.paren, "Expected " +
                                               function.arity() + " arguments but got " +
                                               args.Count + ".");
        }

        return function.call(this, args);
    }
    
    public object VisitUnaryExpr(Expr.Unary expr) {
        object right = Evaluate(expr.right);

        switch (expr.op.Type) {
            case TokenType.MINUS:
                CheckNumberOperand(expr.op, right);
                return -(double)right;
            case TokenType.BANG:
                return !IsTruthy(right);
        }
        
        return null;
    }
    
    private bool IsTruthy(object ob) {
        if (ob == null) return false;
        if (ob is bool) return (bool)ob;
        return true;
    }
    
    private bool IsEqual(Object a, Object b) {
        if (a == null && b == null) return true;
        if (a == null) return false;

        return a.Equals(b);
    }
    
    private string Stringify(object ob) {
        if (ob == null) return "null";

        if (ob is Double) {
            String text = ob.ToString();
            if (text.EndsWith(".0")) {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        return ob.ToString();
    }


    private void CheckNumberOperand(Token op, object right)
    {
        if (right is double) return;
        throw new RuntimeError(op, "Operand must be a number");
    }
    
    private void CheckNumberOperands(Token op, Object left, Object right) 
    {
        if (left is double && right is double) return;

        throw new RuntimeError(op, "Operand must be a number");
    }
}