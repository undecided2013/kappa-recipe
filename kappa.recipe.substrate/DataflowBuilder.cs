using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace kappa.recipe.substrate
{
    public class DataflowBuilder<TInput, TState> where TInput : IBasicInput
    {
        private IDataflow<TInput, TState> dataflow;
        public DataflowBuilder<TInput, TState> Initialize<TDataflow>() where TDataflow : IDataflow<TInput, TState>
        {
            dataflow = Activator.CreateInstance<TDataflow>();
            return this;
        }
        public DataflowBuilder<TInput, TState> SetCancellationToken(CancellationToken cts)
        {
            dataflow.CancellationToken = cts;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetInputGetter(Func<Snapshot<TState>, TInput> inputGetter)
        {
            dataflow.InputGetter = inputGetter;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetStateRetriever(Func<TState> stateRetriever)
        {
            dataflow.StateRetriever = stateRetriever;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetStateInitializer(Func<TState> stateInitializer)
        {
            dataflow.StateInitializer = stateInitializer;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetStateSetter(Action<TState> stateSetter)
        {
            dataflow.StateSetter = stateSetter;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetProcessor(Func<Tuple<TInput, TState>, TState> processor)
        {
            dataflow.Processor = processor;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetSnapshotRetriever(Func<Snapshot<TState>> snapshotRetriever)
        {
            dataflow.SnapshotRetriever = snapshotRetriever;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetSnapshotInitializer(Func<Snapshot<TState>> snapshotInitializer)
        {
            dataflow.SnapshotInitializer = snapshotInitializer;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetSnapshotUpdater(Action<Snapshot<TState>> snapshotUpdater)
        {
            dataflow.SnapshotUpdater = snapshotUpdater;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetStatePublisher(Action<TState> statePublisher)
        {
            dataflow.StatePublisher = statePublisher;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetStartupBehaviour(enStartupBehaviour startUpBehaviour)
        {
            dataflow.StartupBehaviour = startUpBehaviour;
            return this;
        }
        public DataflowBuilder<TInput, TState> SetSnapshotBehaviour(enSnapshotUnits snapShotUnits, int triggerCount)
        {
            dataflow.SnapshotTimer.SnapshotUnits = snapShotUnits;
            dataflow.SnapshotTimer.UnitsToSnapshotAt = triggerCount;
            return this;
        }
        public IDataflow<TInput, TState> Build()
        {
            if (dataflow.StartupBehaviour == enStartupBehaviour.StartAtLastSnapshot)
            {
                if ((dataflow.SnapshotUpdater == null) || (dataflow.SnapshotInitializer == null) || (dataflow.SnapshotRetriever == null))
                    throw new ArgumentNullException("All the State delegates need to be assigned");
            }
            if ((dataflow.StateInitializer == null) || (dataflow.StateRetriever == null) || (dataflow.StateSetter == null))
            {
                throw new ArgumentNullException("Initialize/Set/Retrieve State delegates need to be assigned");
            }
            if (dataflow.InputGetter == null)
            {
                throw new ArgumentNullException("Get Input delegate needs to be assigned");
            }
            if (dataflow.Processor == null)
            {
                throw new ArgumentNullException("Processor delegate needs to be assigned");
            }
            return dataflow;
        }
    }
}
