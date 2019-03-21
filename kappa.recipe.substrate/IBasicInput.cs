using System;
using System.Collections.Generic;
using System.Text;

namespace kappa.recipe.substrate
{
    public interface IBasicInput
    {
        string ID { get; }
        long Timestamp { get; }
    }
}
