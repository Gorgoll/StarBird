using System;

namespace StarBird;

public class RuntimeError : Exception
{
    private readonly Token token;

    public RuntimeError(Token token, string msg) : base(msg)
    {
        this.token = token;
    }
}