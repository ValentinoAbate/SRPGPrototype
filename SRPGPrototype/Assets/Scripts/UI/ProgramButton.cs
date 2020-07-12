using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ProgramButton : MonoBehaviour
{

    public TextMeshProUGUI buttonNameText;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Initialize(Program p, CustUI uiManager)
    {
        buttonNameText.text = p.DisplayName;

        button.onClick.AddListener(() => uiManager.PickupProgram(this, p));
        button.onClick.AddListener(() => button.interactable = false);
    }

    public void Cancel()
    {
        button.interactable = true;
    }
}
