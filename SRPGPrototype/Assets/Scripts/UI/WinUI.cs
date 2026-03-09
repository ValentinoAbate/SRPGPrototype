using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinUI : MonoBehaviour
{
    private void Start()
    {
        UIManager.main.TopBarUI.SetTitleText("You Win!");
    }
    public void OnReturnToTitle()
    {
        SaveManager.ClearRunData();
        PersistantData.main.ResetRunData();
        SceneTransitionManager.main.TransitionToScene(SceneTransitionManager.TitleScene);
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public void ExportSave()
    {
        PopupManager.main.ShowInputPopup(OnInputComplete, null, 14, titleText: "Name File", confirmText: "Export");
    }

    private static void OnInputComplete(string input)
    {
        SaveManager.SaveCompletion(input);
    }
}
