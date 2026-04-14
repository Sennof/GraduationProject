using System.Collections.Generic;
using UnityEngine;

public static class EventBus<T> where T : IEvent
{
    private static readonly HashSet<IEventBinding<T>> Bindings = new HashSet<IEventBinding<T>>();

    public static void Register(EventBinding<T> binding) => Bindings.Add(binding);
    public static void Deregister(EventBinding<T> binding) => Bindings.Remove(binding);

    public static void Raise(T @event)
    {
        var snapshot = new HashSet<IEventBinding<T>>(Bindings);

        foreach (var binding in snapshot)
        {
            if (Bindings.Contains(binding))
            {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }
    }

    private static void Clear()
    {
        Debug.Log($"Clearing {typeof(T).Name} bindings");
        Bindings.Clear();
    }
}