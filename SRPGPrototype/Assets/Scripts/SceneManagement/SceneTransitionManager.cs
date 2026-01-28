using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public const string EncounterSceneName = "Encounter";
    public const string CustSceneName = "Cust";
    public const string StartingShopSceneName = "StartingShop";
    public const string TitleScene = "Title";
    public static SceneTransitionManager main;

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

    public void TransitionToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
