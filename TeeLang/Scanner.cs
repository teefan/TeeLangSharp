namespace TeeLang;

using static TokenType;

internal class Scanner(string source)
{
    private readonly List<Token> _tokens = [];

    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "and", And },
        { "class", Class },
        { "else", Else },
        { "false", False },
        { "for", For },
        { "fun", Fun },
        { "if", If },
        { "nil", Nil },
        { "or", Or },
        { "print", Print },
        { "return", Return },
        { "super", Super },
        { "this", This },
        { "true", True },
        { "var", Var },
        { "while", While }
    };

    private int _start;
    private int _current;
    private int _line = 1;

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            // We are at the beginning of the next lexeme.
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(Eof, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            case '(':
                AddToken(LeftParen);
                break;
            case ')':
                AddToken(RightParen);
                break;
            case '{':
                AddToken(LeftBrace);
                break;
            case '}':
                AddToken(RightBrace);
                break;
            case ',':
                AddToken(Comma);
                break;
            case '.':
                AddToken(Dot);
                break;
            case '-':
                AddToken(Minus);
                break;
            case '+':
                AddToken(Plus);
                break;
            case ';':
                AddToken(Semicolon);
                break;
            case '*':
                AddToken(Star);
                break;
            case '!':
                AddToken(Match('=') ? BangEqual : Bang);
                break;
            case '=':
                AddToken(Match('=') ? EqualEqual : Equal);
                break;
            case '<':
                AddToken(Match('=') ? LessEqual : Less);
                break;
            case '>':
                AddToken(Match('=') ? GreaterEqual : Greater);
                break;
            case '/':
                if (Match('/'))
                {
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                }
                else
                {
                    AddToken(Slash);
                }
                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;
            case '\n':
                _line++;
                break;
            case '"':
                ProcessString();
                break;
            default:
                if (IsDigit(c))
                {
                    ProcessNumber();
                }
                else if (IsAlphabet(c))
                {
                    ProcessIdentifier();
                }
                else
                {
                    TeeLang.Error(_line, "Unexpected character.");
                }
                break;
        }
    }

    private void ProcessIdentifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        var text = source.Substring(_start, _current - _start);
        var type = Keywords.GetValueOrDefault(text, Identifier);

        AddToken(type);
    }

    private void ProcessNumber()
    {
        while (IsDigit(Peek())) Advance();

        // Look for a fractional part.
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // Consume the "."
            Advance();

            while (IsDigit(Peek())) Advance();
        }

        AddToken(Number, Convert.ToDouble(source.Substring(_start, _current - _start)));
    }

    private void ProcessString() {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            TeeLang.Error(_line, "Unterminated string.");
            return;
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        var value = source.Substring(_start + 1, (_current - (_start + 1)) - 1);
        AddToken(String, value);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[_current] != expected) return false;

        _current++;
        return true;
    }

    private char Peek()
    {
        return IsAtEnd() ? '\0' : source[_current];
    }

    private char PeekNext()
    {
        return _current + 1 >= source.Length ? '\0' : source[_current + 1];
    }

    private bool IsAlphabet(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsAlphabet(c) || IsDigit(c);
    }

    private bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    private char Advance()
    {
        return source[_current++];
    }

    private void AddToken(TokenType type, object literal = null)
    {
        var text = source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line));
    }

    private bool IsAtEnd()
    {
        return _current >= source.Length;
    }
}
