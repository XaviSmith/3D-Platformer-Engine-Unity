using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericEventListener<T> : MonoBehaviour
{
    [SerializeField] protected Events eventToListen;
    [SerializeField] protected UnityEvent<T> unityEvents;

    protected virtual void OnEnable()
    {
        EventManager<T>.StartListening(eventToListen.ToString(), InvokeEvent);
    }

    protected virtual void OnDisable()
    {
        EventManager<T>.StopListening(eventToListen.ToString(), InvokeEvent);
    }

    public void InvokeEvent(T param)
    {
        unityEvents?.Invoke(param);
    }


}

//For older implementation
public class GenericEventListener : MonoBehaviour
{
    [SerializeField] protected Events eventToListen;
    [SerializeField] protected UnityEvent unityEvents;

    protected virtual void OnEnable()
    {
        EventManager.StartListening(eventToListen.ToString(), InvokeEvent);
    }

    protected virtual void OnDisable()
    {
        EventManager.StopListening(eventToListen.ToString(), InvokeEvent);
    }

    public void InvokeEvent()
    {
        unityEvents?.Invoke();
    }
}
