
namespace StarBird;

public class StarBird
{
    private static readonly Interpreter Interpreter = new();
    static bool hadError;
    static bool hadRuntimeError = false;
    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: StarBird [script]");
            System.Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }
    
    private static void RunPrompt()
    {
        hadError = false;

        while (true)
        {
            Console.Write("> ");
            string line = Console.ReadLine();

            if (line == null)
                break;

            Run(line);
            hadError = false;
        }
    }
    
    private static void RunFile(string path)
    {
        string source = File.ReadAllText(path);
        Run(source);

        if (hadError) System.Environment.Exit(65);
        if (hadRuntimeError) System.Environment.Exit(70);
    }
    
    private static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        Parser parser = new Parser(tokens);
        List<Stmt> statements = parser.Parse();

        if (hadError) return;
        Interpreter.Interpret(statements);
    }
    
    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        hadError = true;
    }
}

