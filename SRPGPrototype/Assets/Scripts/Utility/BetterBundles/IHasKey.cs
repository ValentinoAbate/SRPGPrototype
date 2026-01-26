using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasKey
{
    public string Key { get; }
#if UNITY_EDITOR
    public bool GenerateKey();
#endif
}
