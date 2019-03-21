using System;

namespace kappa.recipe.substrate
{
    public class Snapshot<TState>
    {
        public TState State { get; set; }
        public string LastMessageID { get; set; }
        public long LastTimeStamp { get; set; }
    }
}