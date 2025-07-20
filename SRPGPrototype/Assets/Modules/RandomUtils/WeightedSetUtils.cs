using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomUtils
{
    public class WeightedSetUtils
    {
        public static WeightedSet<T> GetSetWeightsOptional<T>(IReadOnlyList<T> items, IReadOnlyList<float> weights)
        {
            if(weights.Count > 0)
            {
                return new WeightedSet<T>(items, weights);
            }
            return new WeightedSet<T>(items);
        }
    }
}
