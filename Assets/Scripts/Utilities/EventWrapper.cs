using System;

public class EventWrapper
{
    public event Action call;

    public void Subscribe(Action method) => call += method;
    public void Unsubscribe(Action method) => call -= method;

    public void Call() => call?.Invoke();
}