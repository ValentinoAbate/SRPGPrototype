﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
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
