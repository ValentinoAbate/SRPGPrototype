using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramHatch : MonoBehaviour
{
    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    [SerializeField] private Shell[] shells;
    [SerializeField] private Program[] programs;
}
