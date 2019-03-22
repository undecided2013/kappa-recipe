using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace kappa.recipe.substrate
{
    public enum enStartupBehaviour
    {
        StartAtBeginning = 10,
        StartAtEnd = 20,
        StartAtLastSnapshot = 30
    }

    public abstract class BaseDataflow<TInput, TState> : IDataflow<TInput, TState> where TInput : IBasicInput
    {
        private SnapshotTimer snapShotTimer = new SnapshotTimer();
        public SnapshotTimer SnapshotTimer { get { return snapShotTimer; } }
        public CancellationToken CancellationToken { get; set; }
        public Func<Snapshot<TState>, TInput> InputGetter { get; set; }
        public Func<TState> StateRetriever { get; set; }
        public Action<TState> StateSetter { get; set; }
        public Action<TState> StatePublisher { get; set; }
        public Func<Tuple<TInput, TState>, TState> Processor { get; set; }
        public enStartupBehaviour StartupBehaviour { get; set; }
        public Func<Snapshot<TState>> SnapshotRetriever { get; set; }
        public Func<Snapshot<TState>> SnapshotInitializer { get; set; }
        public Func<TState> StateInitializer { get; set; }
        public Action<Snapshot<TState>> SnapshotUpdater { get; set; }
        protected Snapshot<TState> Initialize()
        {
            Snapshot<TState> result;
            switch (StartupBehaviour)
            {
                case enStartupBehaviour.StartAtBeginning:
                    result = SnapshotInitializer();
                    result.State = StateInitializer();
                    return result;
                // TODO:
                //             case enStartupBehaviour.StartAtEnd:
                //                break;
                case enStartupBehaviour.StartAtLastSnapshot:
                    result = SnapshotRetriever();
                    if (result == null)
                    {
                        result = SnapshotInitializer();
                        result.State = StateInitializer();
                    }
                    StateSetter(result.State);
                    return result;
                default:
                    return null;
            }
        }

        protected abstract void Commence(Snapshot<TState> snap);
        public void Start()
        {
            Snapshot<TState> snap = Initialize();
            Commence(snap);
        }
    }
}
