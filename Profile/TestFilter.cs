using MassTransit;
using MassTransit.Configuration;

namespace Profile;

public class TestFilter<T> : IFilter<ConsumeContext<T>>
    where T : class
{
    private int _counter = 0;

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        _counter++;
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateScope("TestFilter");
        scope.Add("a",  _counter);
    }
}

public class TestSpecification<T> : IPipeSpecification<ConsumeContext<T>>
    where T : class
{
    public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
    {
        builder.AddFilter(new TestFilter<T>());
    }

    public IEnumerable<ValidationResult> Validate()
    {
        return [];
    }
}
