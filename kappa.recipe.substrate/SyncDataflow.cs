using System;
using System.Collections.Generic;
using System.Text;

namespace kappa.recipe.substrate
{
    public class SyncDataflow<TInput, TState> : BaseDataflow<TInput, TState> where TInput : IBasicInput
    {
        protected override void Commence(Snapshot<TState> snap)
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                TInput input = InputGetter(snap); // Get Next Message to process
                if (input != null)
                {
                    TState state = StateRetriever();// Get Internal State
                    if (state == null)
                        state = StateInitializer();
                    state = Processor(new Tuple<TInput, TState>(input, state)); // Process Message
                    StateSetter(state);// Update Internal State
                    StatePublisher(state);// Update Public State
                    snap.LastMessageID = input.MessageID;
                    snap.IncludedMessageIDs.Add(input.MessageID);
                    snap.LastTimeStamp = input.MessageTimestamp;
                    snap.State = state;
                    if (SnapshotTimer.Increment())
                    {
                        SnapshotUpdater(snap); // Save Recovery State
                        snap.IncludedMessageIDs.Clear();
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine($@"{snap.State}");
        }
    }
}
