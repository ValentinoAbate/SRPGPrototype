using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterEventManager : MonoBehaviour
{
    public static EncounterEventManager main;
    public Unit.OnDeath OnUnitDeath { get; set; }

    private void Awake()
    {
        if(main == null)
        {
            main = this;
        }
    }
}
