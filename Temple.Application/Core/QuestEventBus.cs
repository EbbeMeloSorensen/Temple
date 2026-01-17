using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Application.Core;

// Simpel event bus for quest events. Det fungerer lige som på et "bus netværk", hvor events "broadcastes"
// fra en server til alle andre servere på pågældende bus, hvor servere så kan ignorere et event eller agere.
// Her er det blot quests, der notificeres, og de kan så agere ved at ændre tilstand, hvilket
// QuestStatusView objektet overvåger og formidler til de komponenter, der styrer brugergrænsensefladen,
// såsom SiteDataFactory
public sealed class QuestEventBus
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
        var eventType = gameEvent.GetType();

        foreach (var (key, handlers) in _handlers)
        {
            if (!key.IsAssignableFrom(eventType))
                continue;

            foreach (var handler in handlers.Cast<Action<TEvent>>())
            {
                handler(gameEvent);
            }
        }
    }
}