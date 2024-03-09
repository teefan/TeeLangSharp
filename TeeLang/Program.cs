﻿using System.Text;

namespace TeeLang;

static class TeeLang
{
    private static bool _hadError = false;
    
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

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[Line {line}] Error: {where}: {message}");
        _hadError = true;
    }
}
