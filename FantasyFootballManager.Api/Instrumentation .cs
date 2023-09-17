using System.Diagnostics;
using System.Diagnostics.Metrics;

public class Instrumentation : IDisposable
{
    public Instrumentation()
    {
        string? version = typeof(Instrumentation).Assembly.GetName().Version?.ToString();
    }

    public ActivitySource ActivitySource { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
    }
}