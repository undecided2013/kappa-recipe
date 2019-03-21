using System;
using System.Threading;

namespace kappa.recipe.substrate
{
    public interface IDataflow<TInput, TState> where TInput : IBasicInput
    {
        CancellationToken CancellationToken { get; set; }
        Func<Snapshot<TState>, TInput> InputGetter { get; set; }
        Func<Tuple<TInput, TState>, TState> Processor { get; set; }
        Func<Snapshot<TState>> SnapshotRetriever { get; set; }
        Func<Snapshot<TState>> SnapshotInitializer { get; set; }
        SnapshotTimer SnapshotTimer { get; }
        Action<Snapshot<TState>> SnapshotUpdater { get; set; }
        enStartupBehaviour StartupBehaviour { get; set; }
        Action<TState> StatePublisher { get; set; }
        Func<TState> StateRetriever { get; set; }
        Func<TState> StateInitializer { get; set; }
        Action<TState> StateSetter { get; set; }
        void Start();
    }
}