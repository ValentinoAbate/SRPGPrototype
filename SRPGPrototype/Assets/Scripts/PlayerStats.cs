using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int MaxHp { get; set; }

    public int Hp { get; set; }

    public int MaxAP { get; set; }

    public int Repair { get; set; }

    public void RestoreHpToMax() => Hp = MaxHp;
}
