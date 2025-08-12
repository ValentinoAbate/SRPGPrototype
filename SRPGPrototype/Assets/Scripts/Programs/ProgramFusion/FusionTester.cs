using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionTester : MonoBehaviour
{
    [SerializeField] private ProgramFuser fuser;
    [SerializeField] private Program p1;
    [SerializeField] private Program p2;
    [ContextMenu("Fuse")]
    public void Fuse()
    {
        PersistantData.main.inventory.RemoveProgram(p1);
        PersistantData.main.inventory.RemoveProgram(p2);
        PersistantData.main.inventory.AddProgram(fuser.FusePrograms(transform, p1, p2));
    }
}
