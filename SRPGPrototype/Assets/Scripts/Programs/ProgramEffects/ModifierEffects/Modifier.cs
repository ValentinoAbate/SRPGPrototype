using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifier : MonoBehaviour
{
    public enum FilterMode
    { 
        None,
        NeedOne,
        NeedAll,
    }
    [SerializeField] private FilterMode attributeFilterMode = FilterMode.None;
    [SerializeField] private Program.Attributes attributes = Program.Attributes.None;
    public bool AppliesTo(Program p)
    {
        if(attributeFilterMode == FilterMode.NeedOne)
        {
            if ((attributes & p.attributes) == 0)
                return false;
        }
        else if(attributeFilterMode == FilterMode.NeedAll)
        {
            if (!p.attributes.HasFlag(attributes))
                return false;
        }
        return ModFilter(p);
    }

    protected abstract bool ModFilter(Program p);
}
