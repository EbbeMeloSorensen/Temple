using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests;

public sealed class EventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Subscribe<TEvent>(Action<TEvent> handler)
        where TEvent : IGameEvent
    {
        var type = typeof(TEvent);

        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            _handlers[type] = list;
        }

        list.Add(handler);
    }

    public void Publish<TEvent>(TEvent gameEvent)
        where TEvent : IGameEvent
    {
        if (!_handlers.TryGetValue(typeof(TEvent), out var list))
            return;

        foreach (var handler in list.Cast<Action<TEvent>>())
        {
            handler(gameEvent);
        }
    }

    // Optional convenience overload
    public void Publish(IGameEvent gameEvent)
    {
        var type = gameEvent.GetType();

        if (!_handlers.TryGetValue(type, out var list))
            return;

        foreach (var handler in list)
        {
            handler.DynamicInvoke(gameEvent);
        }
    }
}
