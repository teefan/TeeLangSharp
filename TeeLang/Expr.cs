namespace TeeLang;

abstract class Expr
{
    public class Binary(Expr left, Token op, Expr right) : Expr
    {
        public Expr Left { get; } = left;
        public Token Op { get; } = op;
        public Expr Right { get; } = right;
    }
    public class Grouping(Expr expression) : Expr
    {
        public Expr Expression { get; } = expression;
    }
    public class Literal(object value) : Expr
    {
        public object Value { get; } = value;
    }
    public class Unary(Token op, Expr right) : Expr
    {
        public Token Op { get; } = op;
        public Expr Right { get; } = right;
    }
}
