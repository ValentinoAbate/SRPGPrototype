using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject programPrefab;

    [SerializeField] private Button showButton;
    [SerializeField] private TextMeshProUGUI showButtonText;

    private void Start()
    {
        // Find all the programs that have ready upgrades
        // Display the number on the showButton, disable showbutton if none
    }

    public void Show()
    {

    }

    public void Hide()
    {

    }
}
