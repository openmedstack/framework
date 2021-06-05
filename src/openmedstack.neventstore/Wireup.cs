using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using OpenMedStack.NEventStore.Conversion;
using OpenMedStack.NEventStore.Logging;
using OpenMedStack.NEventStore.Persistence;
using OpenMedStack.NEventStore.Persistence.InMemory;

namespace OpenMedStack.NEventStore
{
    using System;

    public class Wireup
    {
        private readonly NanoContainer? _container;
        private readonly Wireup? _inner;
        private readonly ILog _logger = LogFactory.BuildLogger(typeof(Wireup));

        protected Wireup(NanoContainer container)
        {
            _container = container;
        }

        protected Wireup(Wireup inner)
        {
            _inner = inner;
        }

        protected NanoContainer Container => _container ?? _inner!.Container;

        public static Wireup Init()
        {
            var container = new NanoContainer();
            container.Register(TransactionScopeAsyncFlowOption.Enabled);

            container.Register<IPersistStreams>(new InMemoryPersistenceEngine());
            container.Register(BuildEventStore);

            return new Wireup(container);
        }

        public virtual Wireup With<T>(T instance) where T : class
        {
            Container.Register(instance);
            return this;
        }

        public virtual Wireup HookIntoPipelineUsing(IEnumerable<IPipelineHook> hooks) => HookIntoPipelineUsing((hooks ?? Array.Empty<IPipelineHook>()).ToArray());

        public virtual Wireup HookIntoPipelineUsing(params IPipelineHook[] hooks)
        {
            _logger.Info(Resources.WireupHookIntoPipeline, string.Join(", ", hooks.Select(h => h.GetType())));
            ICollection<IPipelineHook> collection = hooks.Where(x => x != null).ToArray();
            Container.Register(collection);
            return this;
        }

        public virtual IStoreEvents Build()
        {
            if (_inner != null)
            {
                return _inner.Build();
            }

            return Container.Resolve<IStoreEvents>() ?? throw new Exception($"Could not resolve {nameof(IStoreEvents)}");
        }

        private static IStoreEvents BuildEventStore(NanoContainer context)
        {
            var concurrency = new OptimisticPipelineHook();

            var upconverter = context.Resolve<EventUpconverterPipelineHook>();

            var hooks = context.Resolve<ICollection<IPipelineHook>>() ?? Array.Empty<IPipelineHook>();
            hooks = upconverter == null ? new IPipelineHook[] { concurrency } : new IPipelineHook[] { concurrency, upconverter! }
                .Concat(hooks)
                .Where(x => x != null)
                .ToArray();

            var persistStreams = context.Resolve<IPersistStreams>() ?? throw new Exception($"Could not resolve {nameof(IPersistStreams)}");
            return new OptimisticEventStore(persistStreams, hooks);
        }
    }
}