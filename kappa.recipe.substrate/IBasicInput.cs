using System;
using System.Collections.Generic;
using System.Text;

namespace kappa.recipe.substrate
{
    public interface IBasicInput
    {
        string MessageID { get; }
        long MessageTimestamp { get; }
        string PayloadID { get; }
    }
}
