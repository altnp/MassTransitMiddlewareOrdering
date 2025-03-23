using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Utils.Messaging;

public class PublisherOutboxProcessorService : BackgroundService
{
    private const int MaxParallelWorkers = 10;
    private readonly PublisherOutbox _channel;
    private readonly IBus _bus;

    public PublisherOutboxProcessorService(PublisherOutbox channel, IBus bus)
    {
        _channel = channel;
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workers = Enumerable.Range(0, MaxParallelWorkers).Select(_ => ProcessMessages(stoppingToken)).ToArray();
        await Task.WhenAll(workers);
    }

    //Needs to support Send and Publish
    private async Task ProcessMessages(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var (type, message) = await _channel.DequeueAsync(stoppingToken);

                if (message != null)
                {
                    var publishMethod = typeof(ISendEndpoint)
                        .GetMethods()
                        .FirstOrDefault(m =>
                            m.Name == "Send"
                            && m.IsGenericMethod
                            && m.GetParameters().Length == 2
                            && m.GetParameters()[0].ParameterType == typeof(object)
                        )
                        ?.MakeGenericMethod(type);

                    if (publishMethod != null)
                    {
                        await (Task)publishMethod.Invoke(_bus, [message, stoppingToken])!;
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
