namespace OpenMedStack.Tests.Startup;

using System;
using Microsoft.Extensions.Logging;

internal class FakeLogger<T> : ILogger<T>
{
    public LogLevel LevelCalled { get; set; }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        LevelCalled = logLevel > LevelCalled ? logLevel : LevelCalled;
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <inheritdoc />
    IDisposable ILogger.BeginScope<TState>(TState state) => throw new NotImplementedException();
}
