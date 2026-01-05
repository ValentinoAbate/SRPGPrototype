using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterEventManager : MonoBehaviour
{
    public static EncounterEventManager main;
    public static bool Ready => main != null;
    public Unit.OnDeath OnUnitDeath { get; set; }
    public Unit.OnDamaged OnUnitDamaged { get; set; }
    public System.Action<Unit> OnUnitSpawned { get; set; }
    public System.Action<Unit> OnUnitRemoved{ get; set; }

    private void Awake()
    {
        if(main == null)
        {
            main = this;
        }
    }
}
