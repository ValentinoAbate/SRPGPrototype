using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramModifier : MonoBehaviour
{
    public abstract bool AppliesTo(Program p);
}
