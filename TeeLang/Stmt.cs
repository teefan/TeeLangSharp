namespace TeeLang;

public abstract class Stmt
{
    public interface IVisitor<out T>
    {
        public T VisitExpressionStmt(Expression stmt);
        public T VisitPrintStmt(Print stmt);
    }

    public class Expression(Expr expression) : Stmt
    {
        public Expr Expr { get; } = expression;

        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }
    public class Print(Expr expression) : Stmt
    {
        public Expr Expr { get; } = expression;

        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    public abstract TR Accept<TR>(IVisitor<TR> visitor);
}
