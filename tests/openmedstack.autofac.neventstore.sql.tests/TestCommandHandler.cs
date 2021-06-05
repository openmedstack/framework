namespace OpenMedStack.Autofac.NEventStore.Sql.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using OpenMedStack;
    using OpenMedStack.Commands;
    using OpenMedStack.Domain;

    internal class TestCommandHandler : CommandHandlerBase<TestCommand>
    {
        /// <inheritdoc />
        public TestCommandHandler(
            IRepository repository,
            IValidateTokens tokenValidator,
            ILogger<TestCommandHandler> logger) : base(repository, tokenValidator, logger)
        {
        }

        /// <inheritdoc />
        protected override async Task<CommandResponse> HandleInternal(
            TestCommand command,
            IMessageHeaders headers,
            CancellationToken cancellationToken)
        {
            try
            {
                var aggregate = await Get<TestAggregate>(command.AggregateId).ConfigureAwait(false);

                aggregate.DoSomething();

                await Save(aggregate).ConfigureAwait(false);

                return command.CreateResponse();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                throw;
            }
        }
    }
}
