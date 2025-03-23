using System.Threading.Channels;

namespace Utils.Messaging;

public class PublisherOutbox
{
    protected readonly Channel<(Type, object)> Channel;

    public PublisherOutbox()
    {
        Channel = System.Threading.Channels.Channel.CreateBounded<(Type, object)>(
            new BoundedChannelOptions(50_000) { FullMode = BoundedChannelFullMode.DropNewest }
        );
    }

    public virtual async Task EnqueueAsync<T>(object message, CancellationToken cancellationToken = default)
        where T : class
    {
        await EnqueueAsync(typeof(T), message, cancellationToken);
    }

    public virtual async Task EnqueueAsync(Type type, object message, CancellationToken cancellationToken = default)
    {
        await Channel.Writer.WriteAsync((type, message), cancellationToken);
    }

    internal virtual async Task<(Type, object)> DequeueAsync(CancellationToken cancellationToken = default)
    {
        return await Channel.Reader.ReadAsync(cancellationToken);
    }
}
