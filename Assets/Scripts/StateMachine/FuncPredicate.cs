using System;

public class FuncPredicate : IPredicate
{
    readonly Func<bool> func; //Func is essentially a delegate that returns a value

    public FuncPredicate(Func<bool> _func)
    {
        func = _func;
    }

    public bool Evaluate() => func.Invoke();
}
