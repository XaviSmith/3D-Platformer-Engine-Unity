using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Predicates are essentially boolean conditions that need to be checked/met
/// </summary>
public interface IPredicate
{
    bool Evaluate();
}