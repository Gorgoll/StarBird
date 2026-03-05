namespace StarBird;

public interface StarBirdCallable
{
    object call(Interpreter interpreter, List<object> arguments);
    int arity();
}