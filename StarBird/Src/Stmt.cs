namespace StarBird;

public abstract class Stmt
{
    public interface IVisitor<R>
    {
        R VisitBlockStmt(Block stmt);
        R VisitExpressionStmt(Expression stmt);
        R VisitPrintStmt(Print stmt);
        R VisitVarStmt(Var stmt);
    }

    public abstract R Accept<R>(IVisitor<R> visitor);
         public sealed class Block : Stmt
    {
        public Block(List<Stmt> statements)
        {
             this.statements = statements;
        }

         public readonly List<Stmt> statements;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }
         public sealed class Expression : Stmt
    {
        public Expression(Expr expression)
        {
             this.expression = expression;
        }

         public readonly Expr expression;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }
         public sealed class Print : Stmt
    {
        public Print(Expr expression)
        {
             this.expression = expression;
        }

         public readonly Expr expression;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }
    public sealed class Var : Stmt
    {
        public Var(Token name, Expr initializer)
        {
             this.name = name;
             this.initializer = initializer;
        }

         public readonly Token name;
         public readonly Expr initializer;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }
    
}
