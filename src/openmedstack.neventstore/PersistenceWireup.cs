using OpenMedStack.NEventStore.Logging;
using OpenMedStack.NEventStore.Persistence;

namespace OpenMedStack.NEventStore
{
    using System;

    public class PersistenceWireup : Wireup
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(PersistenceWireup));
        private bool _initialize;

        public PersistenceWireup(Wireup inner)
            : base(inner)
        {
        }

        public virtual PersistenceWireup WithPersistence(IPersistStreams instance)
        {
            Logger.Info(Messages.RegisteringPersistenceEngine, instance.GetType());
            With(instance);
            return this;
        }

        public virtual PersistenceWireup InitializeStorageEngine()
        {
            Logger.Info(Messages.ConfiguringEngineInitialization);
            _initialize = true;
            return this;
        }

        public override IStoreEvents Build()
        {
            Logger.Info(Messages.BuildingEngine);
            var engine = Container.Resolve<IPersistStreams>() ?? throw new Exception($"Could not resolve {nameof(IPersistStreams)}");

            if (_initialize)
            {
                Logger.Debug(Messages.InitializingEngine);
                engine.Initialize();
            }

            return base.Build();
        }
    }
}
