using System.Reflection;
using Amazon.SQS.Model;
using MassTransit;
using MassTransit.AmazonSqsTransport;

namespace Utils.Middlewares;

public class ConsumerInspectionFilter<T> : IFilter<ConsumeContext<T>>
    where T : class
{
    //Per call
    public ConsumerInspectionFilter() { }

    public void Probe(ProbeContext context)
    {
        // ??
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        try
        {
            var receiveCount = 0;
            if (context.ReceiveContext.Body is SqsMessageBody messageBody)
            {
                // Use reflection to get the private field "_message"
                FieldInfo? fieldInfo = typeof(SqsMessageBody).GetField(
                    "_message",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
                if (fieldInfo != null)
                {
                    var sqsMessage = fieldInfo.GetValue(messageBody) as Message;
                    if (sqsMessage != null)
                    {
                        receiveCount = int.Parse(sqsMessage.Attributes["ApproximateReceiveCount"] ?? "0");
                    }
                }
            }

            var redeliveryCount = context.GetRedeliveryCount();
            var retryAttempt = context.GetRetryAttempt();

            await next.Send(context);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
