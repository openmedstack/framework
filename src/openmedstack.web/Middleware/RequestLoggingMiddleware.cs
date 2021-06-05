namespace OpenMedStack.Web.Middleware
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    internal class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Action<string> _loggingAction;

        public RequestLoggingMiddleware(RequestDelegate next, Action<string> loggingAction)
        {
            _next = next;
            _loggingAction = loggingAction;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var remoteEndPoint = new IPEndPoint(
                  context.Connection.RemoteIpAddress ?? IPAddress.None,
                  context.Connection.RemotePort);
                _loggingAction(
                  $"Connection from {remoteEndPoint} to {context.Request.Path} started {DateTimeOffset.Now.ToUnixTimeMilliseconds()}.");
                await _next(context).ConfigureAwait(false);
                _loggingAction(
                  $"Connection from {remoteEndPoint} to {context.Request.Path} ended {DateTimeOffset.Now.ToUnixTimeMilliseconds()}.");
            }
            catch (Exception e)
            {
                _loggingAction("Exception handling request to " + context.Request.Path);
                _loggingAction(e.Message);
                _loggingAction(e.StackTrace);
                _loggingAction("-------");
            }
        }
    }
}
