using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Threading;

namespace kappa.recipe.substrate.tests
{
    internal class TestInput : IBasicInput
    {
        public int Value;
        public string ID { get; set; }
        public long Timestamp { get; set; }
    }
    internal class TestStateRequest { }
    internal class TestState
    {
        public int TotalValue;
    }
    [TestClass]
    public class SyncDataflowTests
    {
        public bool TestWithInMemData()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            // Dummy Snapshot storage
            String _snapshot = string.Empty;
            ConcurrentBag<String> snapshotbag = new ConcurrentBag<String>();
            String[] messageArray = new string[1000];
            // Populate dummy data
            for (int x = 0; x < 1000; x++)
            {
                TestInput ti = new TestInput() { ID = x.ToString(), Value = 1 };
                messageArray[x] = JsonConvert.SerializeObject(ti);
            }
            // Dummy State Storage
            TestState state = new TestState();
            // Let's build a flow
            DataflowBuilder<TestInput, TestState> dfBuilder =
                new DataflowBuilder<TestInput, TestState>();
            dfBuilder.Initialize<SyncDataflow<TestInput, TestState>>(). // Type of Flow is SyncDataflow
            SetCancellationToken(cts.Token). // Token to be used to cancel flow processing
            SetStartupBehaviour(enStartupBehaviour.StartAtLastSnapshot). // Start processing where we left off
            SetSnapshotBehaviour(enSnapshotUnits.Count, 10). // Snapshot every 60 seconds
            SetInputGetter((Snapshot<TestState> snap) =>  // Get Message from Array
            {
                int lastID = int.Parse(snap.LastMessageID); // Last Message id processed
                                                            // Signal end of input data
                if (lastID >= messageArray.Length - 2)   // Reacged end of Array - Signal end of flow
                {
                    cts.Cancel();
                }
                String msg = messageArray[lastID + 1]; // Get Next Message
                return JsonConvert.DeserializeObject<TestInput>(msg);
            }).
            SetProcessor((Tuple<TestInput, TestState> i) =>  // Process Message
            {
                return new TestState()
                {
                    TotalValue = i.Item2.TotalValue + i.Item1.Value //Simple, just total it up
                };
            }).
            SetSnapshotRetriever(() =>
            {
                return JsonConvert.DeserializeObject<Snapshot<TestState>>(_snapshot); // Use dummy snapshot
            }).
            SetSnapshotUpdater((Snapshot<TestState> snap) =>
            {
                _snapshot = JsonConvert.SerializeObject(snap); // Save snaphsot into dummy
                System.Diagnostics.Debug.WriteLine($"Snap:{snap.LastMessageID}, {snap.State.TotalValue}");
            }).
            SetStatePublisher(s =>
            {
                System.Diagnostics.Debug.WriteLine($"Total:{s.TotalValue}");
            }).
            SetStateRetriever(() => state). // Get State from Dummy
            SetStateInitializer(() => new TestState() { TotalValue = 0 }). // Initialize State if nothing is retrieved
            SetSnapshotInitializer(() => new Snapshot<TestState>() { LastMessageID = "-1", LastTimeStamp = DateTimeOffset.MinValue.ToUnixTimeMilliseconds() }). // Initialize Snapshot if nothing is retrieved
            SetStateSetter(f =>
            {
                state.TotalValue = f.TotalValue; // Update State

            }).Build().Start();
            return true;
        }
        [TestMethod]
        public void TestInMem()
        {
            TestWithInMemData();
        }
    }
}
