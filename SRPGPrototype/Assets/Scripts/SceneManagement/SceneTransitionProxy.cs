using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionProxy : MonoBehaviour
{
    public void TransitionToScene(string sceneName)
    {
        SceneTransitionManager.main.TransitionToScene(sceneName);
    }
}
