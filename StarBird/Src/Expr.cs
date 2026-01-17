namespace StarBird;

public abstract class Expr
{
    public interface IVisitor<R>
    {
        R VisitAssignExpr(Assign expr);
        R VisitBinaryExpr(Binary expr);
        R VisitGroupingExpr(Grouping expr);
        R VisitLiteralExpr(Literal expr);
        R VisitUnaryExpr(Unary expr);
        R VisitVariableExpr(Variable expr);
    }

    public abstract R Accept<R>(IVisitor<R> visitor);
         public sealed class Assign : Expr
    {
        public Assign(Token name, Expr value)
        {
             this.name = name;
             this.value = value;
        }

         public readonly Token name;
         public readonly Expr value;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
    }
         public sealed class Binary : Expr
    {
        public Binary(Expr left, Token op, Expr right)
        {
             this.left = left;
             this.op = op;
             this.right = right;
        }

         public readonly Expr left;
         public readonly Token op;
         public readonly Expr right;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }
         public sealed class Grouping : Expr
    {
        public Grouping(Expr expression)
        {
             this.expression = expression;
        }

         public readonly Expr expression;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }
         public sealed class Literal : Expr
    {
        public Literal(object value)
        {
             this.value = value;
        }

         public readonly object value;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
         public sealed class Unary : Expr
    {
        public Unary(Token op, Expr right)
        {
             this.op = op;
             this.right = right;
        }

         public readonly Token op;
         public readonly Expr right;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
         public sealed class Variable : Expr
    {
        public Variable(Token name)
        {
             this.name = name;
        }

         public readonly Token name;

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }
}
