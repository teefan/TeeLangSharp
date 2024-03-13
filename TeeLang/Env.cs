namespace TeeLang;

public class Env
{
    private readonly Dictionary<string, object> _values = new();

    public object Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Define(string name, object value)
    {
        _values.Add(name, value);
    }
}
