using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
    public string mainMenuSceneName;

    public Text lettersCollectedText;

    public void GoToMainMenu()
    {
        Application.LoadLevel(mainMenuSceneName);
    }

    void Start()
    {
        lettersCollectedText.text = "Letters collected: " + ScoreManager.lettersCollected + " / " + ScoreManager.totalLetters;
    }
}
