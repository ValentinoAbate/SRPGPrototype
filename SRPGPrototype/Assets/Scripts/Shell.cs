using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Pattern custArea = null;
    public List<InstalledProgram> preInstalledPrograms = new List<InstalledProgram>();
    public List<InstalledProgram> programs = new List<InstalledProgram>();

    [System.Serializable]
    public struct InstalledProgram
    {
        public Program program;
        public Vector2Int location;
    }

}
