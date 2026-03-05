namespace StarBird;

public class Return : SystemException
{
    public object Data { get; }

    public Return(object value)
    {
        Data = value;
    }
}