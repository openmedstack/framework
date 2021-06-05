using System;
using System.Globalization;
using System.Threading;

namespace OpenMedStack.NEventStore.Logging
{
    internal static class ExtensionMethods
    {
        private const string MessageFormat = "{0:yyyy/MM/dd HH:mm:ss.ff} - {1} - {2} - {3}";

        public static string FormatMessage(this string message, Type typeToLog, params object?[] values)
        {
            string formattedMessage;
            if (values == null || values.Length == 0)
            {
                //no parameters no need to string format
                formattedMessage = message;
            }
            else
            {
                formattedMessage = string.Format(CultureInfo.InvariantCulture, message, values);
            }
            return string.Format(
                CultureInfo.InvariantCulture,
                MessageFormat,
                SystemTime.UtcNow,
                Thread.CurrentThread.GetName(),
                typeToLog.FullName,
                formattedMessage);
        }

        private static string GetName(this Thread thread) =>
            !string.IsNullOrEmpty(thread.Name)
                ? thread.Name
                : thread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
    }
}