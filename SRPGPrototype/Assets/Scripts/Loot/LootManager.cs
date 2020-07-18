using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    [SerializeField]
    private List<Program> programs = new List<Program>();
    [SerializeField]
    private List<Shell> shells = new List<Shell>();

    public Loot<Shell> shellLoot;
    public Loot<Program> programLoot;

    private void Awake()
    {
        shellLoot = new Loot<Shell>(shells);
        programLoot = new Loot<Program>(programs);
    } 
}
