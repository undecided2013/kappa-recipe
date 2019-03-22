using System;
using System.Collections.Generic;

namespace kappa.recipe.substrate
{
    public class Snapshot<TState>
    {
        private List<string> includedMessageIDs = new List<string>();
        public TState State { get; set; }
        public string LastMessageID { get; set; }
        public long LastTimeStamp { get; set; }
        public List<string> IncludedMessageIDs
        {
            get
            {
                return includedMessageIDs;
            }
        }
    }
}