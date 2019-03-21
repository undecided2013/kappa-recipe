using System;
using System.Collections.Generic;
using System.Text;

namespace kappa.recipe.substrate
{
    public enum enSnapshotUnits { Millis = 10, Count = 100 }
    public class SnapshotTimer
    {
        private int count = 0;
        private long lastSnap = 0;
        public int UnitsToSnapshotAt { get; set; }
        public enSnapshotUnits SnapshotUnits { get; set; }

        public bool Increment()
        {
            count++;
            if (SnapshotUnits == enSnapshotUnits.Count)
            {
                if (count > UnitsToSnapshotAt)
                {
                    count = 0;
                    return true;
                }
            }
            else
            {
                if (lastSnap == 0)
                {
                    lastSnap = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
                long elapsedMs = DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastSnap;
                if (elapsedMs > UnitsToSnapshotAt)
                {
                    lastSnap = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    return true;
                }
            }
            return false;
        }
    }
}
