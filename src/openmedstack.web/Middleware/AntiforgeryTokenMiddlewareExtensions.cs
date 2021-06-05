namespace OpenMedStack.Web.Middleware
{
    using Microsoft.AspNetCore.Builder;

    public static class AntiforgeryTokenMiddlewareExtensions
  {
    public static IApplicationBuilder UseAntiforgeryToken(this IApplicationBuilder builder) => builder.UseMiddleware<AntiforgeryTokenMiddleware>();
  }
}