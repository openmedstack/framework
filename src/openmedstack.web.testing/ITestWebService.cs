namespace OpenMedStack.Web.Testing
{
    using System.Net.Http;

    public interface ITestWebService : IService
    {
        HttpClient CreateClient();
    }
}
