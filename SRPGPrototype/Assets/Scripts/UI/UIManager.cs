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

    public TurnOrderViewerUI TurnOrderUI => turnOrderUI;
    [SerializeField] private TurnOrderViewerUI turnOrderUI;

    public UnitUIViewerUI UnitUIViewer => unitUIViewer;
    [SerializeField] private UnitUIViewerUI unitUIViewer;

    public ItemSelector ItemSelector => itemSelector;
    [SerializeField] private ItemSelector itemSelector;

    public ProgramFusionUI ProgramFusionUI => programFusionUI;
    [SerializeField] private ProgramFusionUI programFusionUI;

    [SerializeField] private ShellViewerControllerUI shellViewerController;
    [SerializeField] private GameObject floatTextPrefab;
    [SerializeField] private Transform fxContainer;

    private PrefabPool<FloatText> floatTextPool;

    public BattleUI BattleUI 
    {
        get
        {
            if (battleUI == null)
                return null;
            return battleUI;
        } 
        set => battleUI = value; 
    }
    private BattleUI battleUI;

    private void Awake()
    {
        if(main == null)
        {
            main = this;
            floatTextPool = new PrefabPool<FloatText>(floatTextPrefab, fxContainer, 5);
            DontDestroyOnLoad(transform);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateShellViewerController()
    {
        shellViewerController.Setup();
    }

    public void RemoveUnitShellFromViewer(Shell s)
    {
        shellViewerController.RemoveUnitView(s);
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

    private const float floatTime = 0.5f;
    public void PlayFloatText(Vector2 pos, string text, Color color, System.Action onComplete = null)
    {
        var floatText = floatTextPool.Get();
        void OnComplete()
        {
            floatTextPool.Release(floatText);
            onComplete?.Invoke();
        }
        floatText.Play(Camera.main.WorldToScreenPoint(pos), text, color, floatTime, OnComplete);
    }
}
