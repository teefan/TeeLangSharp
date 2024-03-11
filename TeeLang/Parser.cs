namespace TeeLang;

using static TokenType;

internal class Parser(List<Token> tokens)
{
    private class ParseError : Exception;

    private int _current;

    public Expr Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError error)
        {
            return null;
        }
    }

    private Expr Expression()
    {
        return Equality();
    }

    private Expr Equality()
    {
        var expr = Comparision();

        while (Match(BangEqual, EqualEqual))
        {
            var op = Previous();
            var right = Comparision();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Comparision()
    {
        var expr = Term();

        while (Match(Greater, GreaterEqual, Less, LessEqual))
        {
            var op = Previous();
            var right = Term();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Term()
    {
        var expr = Factor();

        while (Match(Minus, Plus))
        {
            var op = Previous();
            var right = Factor();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Factor()
    {
        var expr = Unary();

        while (Match(Slash, Star))
        {
            var op = Previous();
            var right = Unary();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(Bang, Minus))
        {
            var op = Previous();
            var right = Unary();
            return new Expr.Unary(op, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(False)) return new Expr.Literal(false);
        if (Match(True)) return new Expr.Literal(true);
        if (Match(Nil)) return new Expr.Literal(null);

        if (Match(Number, String))
        {
            return new Expr.Literal(Previous().Literal);
        }

        if (Match(LeftParen))
        {
            var expr = Expression();
            Consume(RightParen, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek(), "Expected expression.");
    }

    private bool Match(params TokenType[] types)
    {
        if (types.Any(Check))
        {
            Advance();
            return true;
        }

        return false;
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == Eof;
    }

    private Token Peek()
    {
        return tokens[_current];
    }

    private Token Previous()
    {
        return tokens[_current - 1];
    }

    private ParseError Error(Token token, string message)
    {
        TeeLang.Error(token, message);
        return new ParseError();
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == Semicolon) return;

            switch (Peek().Type)
            {
                case Class:
                case Fun:
                case Var:
                case For:
                case If:
                case While:
                case Print:
                case Return:
                    return;
            }

            Advance();
        }
    }
}
