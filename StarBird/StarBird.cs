namespace StarBird;

public class StarBird
{
    static bool hadError = false;
    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: StarBird [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            runFile(args[0]);
        }
        else
        {
            runPrompt();
        }
    }
    
    private static void runPrompt()
    {
        hadError = false;

        while (true)
        {
            Console.Write("> ");
            string line = Console.ReadLine();

            if (line == null)
                break;

            run(line);
            hadError = false;
        }
    }
    
    private static void runFile(string path)
    {
        string source = File.ReadAllText(path);
        run(source);

        if (hadError) Environment.Exit(65);
    }
    
    private static void run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.scanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    
    public static void error(int line, string message)
    {
        report(line, "", message);
    }

    private static void report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        hadError = true;
    }
}

