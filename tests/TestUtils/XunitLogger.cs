using System.Globalization;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace TestUtils;

public class XunitLogger : ILogger
{
    private readonly ITestOutputHelper _output;
    public XunitLogger(ITestOutputHelper output)
    {
        _output = output;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var date = DateTimeOffset.Now;
        _output.WriteLine($"{date.ToString("T", CultureInfo.InvariantCulture)}|{logLevel}|{formatter(state, exception)}");
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => EmptyDisposable.Instance;

    private sealed class EmptyDisposable : IDisposable
    {
        public static readonly IDisposable Instance = new EmptyDisposable();
        private EmptyDisposable() { }
        public void Dispose() { }
    }
}
