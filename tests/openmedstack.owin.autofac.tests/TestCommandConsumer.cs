namespace OpenMedStack.Web.Autofac.Tests
{
    using System.Threading.Tasks;
    using MassTransit;
    using OpenMedStack.Commands;

    internal class TestCommandConsumer : IConsumer<TestCommand>
    {
        private readonly IHandleCommands<TestCommand> _handler;

        public TestCommandConsumer(IHandleCommands<TestCommand> handler)
        {
            _handler = handler;
        }

        /// <inheritdoc />
        public async Task Consume(ConsumeContext<TestCommand> context)
        {
            var response = await _handler.Handle(
                    context.Message,
                    new OpenMedStack.MessageHeaders(context.Headers.GetAll()),
                    context.CancellationToken)
                .ConfigureAwait(false);

            await context.RespondAsync(response).ConfigureAwait(false);
        }
    }
}
