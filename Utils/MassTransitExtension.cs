namespace MassTransit;

public static class ISendEndpointProviderExtensions
{
    public static async Task<ISendEndpoint> GetSendEndpoint<T>(this ISendEndpointProvider sendEndpointProvider)
        where T : class
    {
        if (EndpointConvention.TryGetDestinationAddress<T>(out var endpointAddress))
        {
            return await sendEndpointProvider.GetSendEndpoint(endpointAddress);
        }

        throw new EndpointNotFoundException("Unable to resolve endpoint from convention");
    }
}
