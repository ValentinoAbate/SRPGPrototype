using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Button button;
    public EventTrigger trigger;
    public void Initialize(Action action)
    {
        nameText.text = action.DisplayName;
    }
}
