using MassTransit.AmazonSqsTransport;

namespace MassTransit;

public class MyEntityNameFormatter : IEntityNameFormatter
{
    private IEntityNameFormatter _innerFormatter;

    public MyEntityNameFormatter(string scope)
    {
        _innerFormatter = new PrefixEntityNameFormatter(
            new MessageNameFormatterEntityNameFormatter(
                new AmazonSqsMessageNameFormatter(
                    namespaceSeparator: "-",
                    nestedTypeSeparator: "",
                    genericTypeSeparator: "--",
                    genericArgumentSeparator: "---"
                )
            ),
            $"mt-{scope.ToLowerInvariant()}_"
        );
    }

    public string FormatEntityName<T>()
    {
        //TODO add `-` between namespace components
        return _innerFormatter.FormatEntityName<T>().ToLowerInvariant();
    }
}
