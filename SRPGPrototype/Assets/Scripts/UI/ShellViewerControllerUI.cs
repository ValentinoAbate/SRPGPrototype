using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShellViewerControllerUI : MonoBehaviour
{
    [SerializeField] private OtherUnitShellViewer unitViewerPrefab;

    private List<OtherUnitShellViewer> otherUnitShellViewers = new List<OtherUnitShellViewer>();

    public void Start()
    {
        Setup();
    }

    public void RemoveUnitView(Shell s)
    {
        foreach(var viewer in otherUnitShellViewers)
        {
            if(viewer.Shell == s)
            {
                viewer.Cleanup();
            }
        }
    }

    public void Setup()
    {
        var droneShells = new List<Shell>(PersistantData.main.inventory.DroneShells);
        for (int i = 0; i < droneShells.Count; ++i)
        {
            if(i >= otherUnitShellViewers.Count)
            {
                otherUnitShellViewers.Add(Instantiate(unitViewerPrefab, transform));
            }
            otherUnitShellViewers[i].Initialize(droneShells[i], i + 2);
        }
        for(int i = droneShells.Count; i < otherUnitShellViewers.Count; ++i)
        {
            otherUnitShellViewers[i].Cleanup();
        }
    }
}
