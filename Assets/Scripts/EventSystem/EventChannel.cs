using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer;

/// <summary>
/// Follows the Observer pattern.
/// FloatEventChannel and IntEventChannel are separate classes due Unity needing MonoBehaviours to be in a file of their own name
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EventChannel<T> : ScriptableObject
{
    readonly HashSet<EventListener<T>> observers = new HashSet<EventListener<T>>();

    public void Invoke(T value)
    {
        foreach(EventListener<T> observer in observers)
        {
            observer.Raise(value);
        }
    }

    public void Register(EventListener<T> observer) => observers.Add(observer);
    public void Deregister(EventListener<T> observer) => observers.Remove(observer);
}

public readonly struct Empty { } //struct so we can set up a null eventChannel

[CreateAssetMenu(menuName = "Events/EventChannel")]
public class EventChannel : EventChannel<Empty> { }