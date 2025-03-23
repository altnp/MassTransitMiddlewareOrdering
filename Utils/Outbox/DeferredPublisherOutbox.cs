using System.Collections.Concurrent;

namespace Utils.Messaging;

public class DeferredPublisherOutbox
{
    private readonly ConcurrentBag<(Type, object)> _messages = [];
    private readonly PublisherOutbox _outbox;

    public DeferredPublisherOutbox(PublisherOutbox outbox)
    {
        _outbox = outbox;
    }

    public Task EnqueueAsync<T>(object message, CancellationToken cancellationToken = default)
        where T : class
    {
        _messages.Add((typeof(T), message));
        return Task.CompletedTask;
    }

    internal Task<(Type, object)> DequeueAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    internal async Task ReleaseMessages(CancellationToken cancellationToken)
    {
        foreach (var message in _messages)
        {
            await _outbox.EnqueueAsync(message.Item1, message.Item2);
        }
    }
}
