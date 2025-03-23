using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Utils.Messaging;

public class DeferredPublisherOutboxMiddleware
{
    private readonly RequestDelegate _next;

    public DeferredPublisherOutboxMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        //Will not publish if exception is thrown
        var outbox = context.RequestServices.GetService<DeferredPublisherOutbox>();
        if (outbox != null)
        {
            await outbox.ReleaseMessages(context.RequestAborted);
        }
    }
}
