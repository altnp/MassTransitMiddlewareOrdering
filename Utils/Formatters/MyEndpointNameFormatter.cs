namespace MassTransit;

public class MyEndpointNameFormatter : DefaultEndpointNameFormatter
{
    public MyEndpointNameFormatter(string scope)
        : base("-", $"mt-{scope.ToLowerInvariant()}_", true) { }

    protected override string FormatName(Type type)
    {
        return base.FormatName(type).ToLowerInvariant();
    }
}
