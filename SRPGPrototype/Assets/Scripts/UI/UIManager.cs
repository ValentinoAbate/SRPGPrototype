using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    public TopBarUI TopBarUI => topBarUI;
    [SerializeField] private TopBarUI topBarUI;
    public ActionDescriptionUI ActionDescriptionUI => actionDescriptionUI;
    [SerializeField] private ActionDescriptionUI actionDescriptionUI;
    public ProgramDescriptionUI ProgramDescriptionUI => programDescriptionUI;
    [SerializeField] private ProgramDescriptionUI programDescriptionUI;
    public ShellDescriptionUI ShellDescriptionUI => shellDescriptionUI;
    [SerializeField] private ShellDescriptionUI shellDescriptionUI;

    public UnitDescriptionUI UnitDescriptionUI => unitDescriptionUI;
    [SerializeField] private UnitDescriptionUI unitDescriptionUI;

    private void Awake()
    {
        if(main == null)
        {
            main = this;
            DontDestroyOnLoad(transform);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HideShellDescriptionUI(BaseEventData _)
    {
        ShellDescriptionUI.Hide();
    }

    public void HideProgramDescriptionUI(BaseEventData _)
    {
        ProgramDescriptionUI.Hide();
    }

    public void HideActionDescriptionUI(BaseEventData _)
    {
        ActionDescriptionUI.Hide();
    }

    public void HideUnitDescriptionUI(Vector2Int _)
    {
        UnitDescriptionUI.Hide();
    }

    public void HideAllDescriptionUI()
    {
        ShellDescriptionUI.Hide();
        ProgramDescriptionUI.Hide();
        ActionDescriptionUI.Hide();
        UnitDescriptionUI.Hide();
    }
}
