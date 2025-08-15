using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CostButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CostDisplay costDisplay;

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
    }
}
