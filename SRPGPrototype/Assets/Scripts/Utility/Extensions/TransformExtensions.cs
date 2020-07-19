using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static void DestroyAllChildren(this Transform t)
    {
        for (int child = 0; child < t.childCount; ++child)
        {
            GameObject.Destroy(t.GetChild(child).gameObject);
        }

    }
}
