namespace OpenMedStack.Web.Middleware
{
    using System;
    using Microsoft.AspNetCore.Builder;

    public static class RequestLoggingMiddlewareExtensions
  {
    public static void UseRequestLogging(this IApplicationBuilder app, Action<string> loggingAction)
    {
      app.UseMiddleware<RequestLoggingMiddleware>(loggingAction);
    }
  }
}