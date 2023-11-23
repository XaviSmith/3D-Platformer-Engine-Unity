using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Mix of EventChannels and my own Custom EventManager. Subscribe to events by name in a class' OnEnable, unsubscribe in OnDisable.
/// Works with strings and enums.
/// <para>e.g. EventManager.StartListening("EventName", Callback) or EventManager.StartListening(Events.NAME.ToString(), Callback)</para>
/// </summary>
public static class EventManager
{
    private static Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();

    //Subscribe to event
    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    //unsubscribe
    public static void StopListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    //Trigger event for all subscribers
    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

}

/// <summary>
/// Version of EventManager that allows for params e.g. EventManager<int>.StartListening(Events.NAME.ToString(), intFunction)
/// </summary>
/// <typeparam name="T"></typeparam>
public static class EventManager<T>
{
    //The values are Actions() with a dictionary<paramName, value> for the variables
    private static Dictionary<string, Action<T>> eventDictionary = new Dictionary<string, Action<T>>();

    public static void StartListening(string eventName, Action<T> listener)
    {
        Action<T> thisEvent;

        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action<T> listener)
    {
        Action<T> thisEvent;

        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {                     
            thisEvent -=listener;
            eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName, T parameters)
    {
        Action <T> thisEvent = null;

        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {       
            thisEvent.Invoke(parameters);
        }
    }

}


