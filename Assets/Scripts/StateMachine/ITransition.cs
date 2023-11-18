using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles moving between states based on predicates
public interface ITransition
{
    IState ToState { get; }
    IPredicate Predicate { get; }
}
