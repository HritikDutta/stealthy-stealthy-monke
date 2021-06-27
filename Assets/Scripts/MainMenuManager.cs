using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public string tutorialSceneName;
    public string gameSceneName;
    public string narrative;
    public bool skipTutorial = false;

    public void TutorialToggle(bool value)
    {
        skipTutorial = value;
    }

    public void StartGame()
    {
        ScoreManager.Reset();

        if (skipTutorial)
            ScoreManager.nextLevel = gameSceneName;
        else
            ScoreManager.nextLevel = tutorialSceneName;

        Application.LoadLevel(narrative);
    }
}
