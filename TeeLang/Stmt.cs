namespace TeeLang;

public abstract class Stmt
{
    public interface IVisitor<out T>
    {
        public T VisitExpressionStmt(Expression stmt);
        public T VisitPrintStmt(Print stmt);
        public T VisitVarStmt(Var stmt);
    }

    public class Expression(Expr expr) : Stmt
    {
        public Expr Expr { get; } = expr;

        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }
    public class Print(Expr expr) : Stmt
    {
        public Expr Expr { get; } = expr;

        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }
    public class Var(Token name, Expr initializer) : Stmt
    {
        public Token Name { get; } = name;
        public Expr Initializer { get; } = initializer;

        public override TR Accept<TR>(IVisitor<TR> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }

    public abstract TR Accept<TR>(IVisitor<TR> visitor);
}
