using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Pattern custArea;
    public List<InstalledProgram> programs;

    [System.Serializable]
    public struct InstalledProgram
    {
        public Program program;
        public Vector2 location;
    }

}
