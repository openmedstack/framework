using System;
using OpenMedStack.NEventStore.Logging;

namespace OpenMedStack.NEventStore
{
    public static class LoggingWireupExtensions
    {
        public static Wireup LogToConsoleWindow(this Wireup wireup, LogLevel logLevel = LogLevel.Info)
        {
            return wireup.LogTo(type => new ConsoleWindowLogger(type, logLevel));
        }

        public static Wireup LogToOutputWindow(this Wireup wireup, LogLevel logLevel = LogLevel.Info)
        {
            return wireup.LogTo(type => new OutputWindowLogger(type, logLevel));
        }

        public static Wireup LogTo(this Wireup wireup, Func<Type, ILog> logger)
        {
            LogFactory.BuildLogger = logger;
            return wireup;
        }
    }
}