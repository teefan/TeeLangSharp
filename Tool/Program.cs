using System.Globalization;
using System.Text;

namespace Tool;

public class GenerateAst
{
    public static void Main(string[] args)
    {
        var argsLength = args.Length;
        if (argsLength != 1)
        {
            Console.Error.WriteLine("Usage: GenerateAst.exe <output directory>");
            Environment.Exit(64);
        }

        var outputDir = args[0];
        DefineAst(outputDir, "Expr", [
            "Binary   : Expr left, Token op, Expr right",
            "Grouping : Expr expression",
            "Literal  : object value",
            "Unary    : Token op, Expr right"
        ]);
    }

    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        var path = $"{outputDir}/{baseName}.cs";
        var fileContent = new List<string>();

        fileContent.AddRange([
            "namespace TeeLang;",
            "",
            $"abstract class {baseName}",
            "{"
        ]);

        foreach (var type in types)
        {
            var className = type.Split(":")[0].Trim();
            var fields = type.Split(":")[1].Trim();
            DefineType(fileContent, baseName, className, fields);
        }

        fileContent.AddRange([
            "}",
            ""
        ]);

        File.WriteAllText(path, string.Join("\n", fileContent.ToArray()), Encoding.Default);
    }

    private static void DefineType(List<string> fileContent, string baseName, string className, string fieldList)
    {
        fileContent.AddRange([
            $"    public class {className}({fieldList}) : {baseName}",
            "    {"
        ]);

        // Store parameters in fields.
        var fields = fieldList.Split(", ");

        // Fields.
        foreach (var field in fields)
        {
            var type = field.Split(" ")[0];
            var name = field.Split(" ")[1];
            var nameTitleCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
            fileContent.Add($"        public {type} {nameTitleCase} {{ get; }} = {name};");
        }

        fileContent.Add("    }");
    }
}
