namespace StarBird;

public class StarBirdFunction : StarBirdCallable
{
    private readonly Stmt.Function declaration;
    private readonly Environment closure;
    
    public StarBirdFunction(Stmt.Function declaration, Environment closure)
    {
        this.closure = closure;
        this.declaration = declaration;
    }

    public object call(Interpreter interpreter, List<object> arguments)
    {
        Environment environment = new Environment(closure);

        for (int i = 0; i < declaration.param.Count; i++)
        {
            environment.Define(
                declaration.param[i].Lexeme,
                arguments[i]
            );
        }
        try
        {
            interpreter.ExecuteBlock(declaration.body, environment);
        }
        catch (Return returnValue)
        {
            return returnValue.Data;
        }
        return null;
    }

    public int arity()
    {
        return declaration.param.Count;
    }

    public string toString()
    {
        return "<fn " + declaration.name.Lexeme + ">";
    }
}