using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagingActionEffect
{
    public bool DealsDamage { get; }
    public int BaseDamage(Action action, SubAction sub, Unit user, Queue<int> indices);
}
