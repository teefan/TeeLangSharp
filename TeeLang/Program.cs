using System.Text;

namespace TeeLang;

public static class TeeLang
{
    private static readonly Interpreter Interpreter = new Interpreter();
    private static bool _hadError;
    private static bool _hadRuntimeError;

    private static void Main(string[] args)
    {
        var argsLength = args.Length;
        switch (argsLength)
        {
            case > 1:
                Console.WriteLine("Usage: TeeLang [script]");
                Environment.Exit(64);
                break;
            case 1:
                RunFile(args[0]);
                break;
            default:
                RunPrompt();
                break;
        }
    }

    private static void RunFile(string path)
    {
        var source = File.ReadAllText(path, Encoding.Default);
        Run(source);
        
        // Indicate an error in the exit code.
        if (_hadError) Environment.Exit(65);
        if (_hadRuntimeError) Environment.Exit(70);
    }

    private static void RunPrompt()
    {
        for (;;)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null) break;
            Run(line);
            _hadError = false;
        }
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var statements = parser.Parse();

        // Stop if there was a syntax error.
        if (_hadError) return;

        Interpreter.Interpret(statements);
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine($"{error.Message}\n[Line {error.Token.Line}]");
        _hadRuntimeError = true;
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[Line {line}] Error: {where}: {message}");
        _hadError = true;
    }

    public static void Error(Token token, string message)
    {
        Report(token.Line, token.Type == TokenType.Eof ? " at end" : $" at '{token.Lexeme}'", message);
    }
}
