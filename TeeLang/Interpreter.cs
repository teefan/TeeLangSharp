﻿namespace TeeLang;

using static TokenType;

public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    private Env _env = new();

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            TeeLang.RuntimeError(error);
        }
    }

    public object VisitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case Greater:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left > (double)right;
            case GreaterEqual:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left >= (double)right;
            case Less:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left < (double)right;
            case LessEqual:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left <= (double)right;
            case BangEqual:
                return !IsEqual(left, right);
            case EqualEqual:
                return IsEqual(left, right);
            case Minus:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left - (double)right;
            case Plus:
                if (left is double l && right is double r)
                {
                    return l + r;
                }

                if (left is string sl && right is string sr)
                {
                    return sl + sr;
                }

                throw new RuntimeError(expr.Op, "Operands must be two numbers or two strings.");
            case Slash:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left / (double)right;
            case Star:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left * (double)right;
        }

        // Unreachable.
        return null;
    }

    public object VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expr);
    }

    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value;
    }

    public object VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case Bang:
                return !IsTruthy(right);
            case Minus:
                return -(double)right;
        }

        // Unreachable.
        return null;
    }

    public object VisitVariableExpr(Expr.Variable expr)
    {
        return _env.Get(expr.Name);
    }

    private void CheckNumberOperand(Token op, object operand)
    {
        if (operand is double) return;

        throw new RuntimeError(op, "Operand must be a number.");
    }

    private void CheckNumberOperands(Token op, object left, object right)
    {
        if (left is double && right is double) return;

        throw new RuntimeError(op, "Operands must be numbers.");
    }

    private bool IsTruthy(object obj)
    {
        if (obj == null) return false;
        if (obj is bool b) return b;
        return true;
    }

    private bool IsEqual(object a, object b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;

        return a.Equals(b);
    }

    private string Stringify(object obj)
    {
        if (obj == null) return "nil";

        if (obj is double)
        {
            var text = obj.ToString();
            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return text;
        }

        return obj.ToString();
    }

    private object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }

    public object VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object VisitPrintStmt(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Expr);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object VisitVarStmt(Stmt.Var stmt)
    {
        object value = null;
        if (stmt.Initializer != null)
        {
            value = Evaluate(stmt.Initializer);
        }

        _env.Define(stmt.Name.Lexeme, value);
        return null;
    }
}
