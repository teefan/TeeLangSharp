namespace TeeLang;

public abstract class Expr
{
    public interface IVisitor<out T>
    {
        public T VisitBinaryExpr(Binary expr);
        public T VisitGroupingExpr(Grouping expr);
        public T VisitLiteralExpr(Literal expr);
        public T VisitUnaryExpr(Unary expr);
    }

    public class Binary(Expr left, Token op, Expr right) : Expr
    {
        public Expr Left { get; } = left;
        public Token Op { get; } = op;
        public Expr Right { get; } = right;

        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }
    public class Grouping(Expr expression) : Expr
    {
        public Expr Expression { get; } = expression;

        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }
    public class Literal(object value) : Expr
    {
        public object Value { get; } = value;

        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
    public class Unary(Token op, Expr right) : Expr
    {
        public Token Op { get; } = op;
        public Expr Right { get; } = right;

        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    public abstract TR Accept<TR>(IVisitor<TR> visitor);
}
