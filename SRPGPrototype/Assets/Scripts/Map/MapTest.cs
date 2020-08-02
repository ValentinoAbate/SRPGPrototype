using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTest : MonoBehaviour
{
    public MapGenerator generator;
    public MapData data;
    // Start is called before the first frame update
    void Start()
    {
        var map = generator.Generate(data);
    }
}
