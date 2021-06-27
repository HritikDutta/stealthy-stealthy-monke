using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public string tutorialSceneName;
    public string gameSceneName;
    public bool skipTutorial = false;

    public void TutorialToggle(bool value)
    {
        skipTutorial = value;
    }

    public void StartGame()
    {
        ScoreManager.Reset();

        if (skipTutorial)
            Application.LoadLevel(gameSceneName);
        else
            Application.LoadLevel(tutorialSceneName);
    }
}
