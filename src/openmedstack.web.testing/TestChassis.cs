namespace OpenMedStack.Web.Testing
{
    using System;
    using System.Net.Http;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using OpenMedStack.Web.Autofac;

    /// <summary>
    /// Defines the chassis for tests
    /// </summary>
    public class TestChassis<TConfiguration> : IDisposable
        where TConfiguration : WebDeploymentConfiguration
    {
        private readonly Chassis<TConfiguration> _chassis;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestChassis{T}"/> class.
        /// </summary>
        /// <param name="chassis"></param>
        public TestChassis(Chassis<TConfiguration> chassis)
        {
            _chassis = chassis;
        }

        /// <summary>
        /// Starts the chassis.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The started <see cref="ITestWebService"/> as an asynchronous operation.</returns>
        public IAsyncDisposable Start(CancellationToken cancellationToken = default) =>
            _chassis.Start(cancellationToken);

        public HttpClient CreateClient() => _chassis.Resolve<HttpClient>();

        /// <inheritdoc />
        public void Dispose()
        {
            _chassis.Dispose();
            GC.SuppressFinalize(this);
        }

        public IDisposable Subscribe(Action<object> func) =>
            _chassis.SubscribeOn(TaskPoolScheduler.Default).Subscribe(func);
    }
}
