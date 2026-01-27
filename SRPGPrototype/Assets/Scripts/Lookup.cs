using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookup : MonoBehaviour
{
    private static Lookup instance;

    [SerializeField] private UnitBundle units;
    [SerializeField] private MapDataBundle maps;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static bool TryGetUnit(string key, out Unit unit) => instance.units.TryGet(key, out unit);
    public static bool TryGetMap(string key, out MapData map) => instance.maps.TryGet(key, out map);
}
