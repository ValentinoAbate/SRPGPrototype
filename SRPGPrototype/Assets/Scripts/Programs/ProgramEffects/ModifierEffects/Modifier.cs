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
    [SerializeField] protected string displayNameOverride;

    public virtual string DisplayName
    {
        get
        {
            if (!string.IsNullOrEmpty(displayNameOverride))
                return displayNameOverride;
            var prog = Program;
            if (prog == null)
                return string.Empty;
            return prog.DisplayName;
        }
    }

    public Program Program
    {
        get
        {
            if (program == null)
            {
                program = GetComponent<Program>();
            }
            return program;
        }
    }
    private Program program;

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
