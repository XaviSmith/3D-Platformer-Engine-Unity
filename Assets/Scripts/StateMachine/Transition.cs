using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : ITransition
{
    public IState ToState { get; }
    public IPredicate Predicate { get; }

    public Transition(IState _to, IPredicate _predicate)
    {
        ToState = _to;
        Predicate = _predicate;
    }
}
