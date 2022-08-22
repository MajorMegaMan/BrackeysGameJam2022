using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    // returns true if tarhet layer is found in includelayers
    public static bool ContainsLayer(int targetLayer, int includeLayers)
    {
        return ((1 << targetLayer) & includeLayers) != 0;
    }

    // returns true if target layer is not found in ignore layers
    public static bool IgnoresLayer(int targetLayer, int ignoreLayers)
    {
        return ((1 << targetLayer) & ignoreLayers) == 0;
    }
}
