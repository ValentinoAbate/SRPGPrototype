using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookup : MonoBehaviour
{
    private static Lookup instance;

    [SerializeField] private ProgramBundle programs;
    [SerializeField] private ShellBundle shells;
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

    public static bool TryGetProgram(string key, out Program program) => instance.programs.TryGet(key, out program);
    public static bool TryGetShell(string key, out Shell shell) => instance.shells.TryGet(key, out shell);
    public static bool TryGetUnit(string key, out Unit unit) => instance.units.TryGet(key, out unit);
    public static bool TryGetMap(string key, out MapData map) => instance.maps.TryGet(key, out map);
}
