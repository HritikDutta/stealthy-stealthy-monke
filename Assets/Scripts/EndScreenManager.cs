using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
    public string mainMenuSceneName;

    public Text lettersCollectedText;
    public Text casualtiesText;
    public Text demonsWereAlertedText;

    public Text scoreText;

    int score = 0;

    public void GoToMainMenu()
    {
        Application.LoadLevel(mainMenuSceneName);
    }

    void Start()
    {
        lettersCollectedText.text =  "Letters collected (" + ScoreManager.lettersCollected.ToString() + ") : +" + (ScoreManager.lettersCollected * 100).ToString();
        casualtiesText.text =        "Casualties (" + ScoreManager.casualties.ToString() + ") : -" + (ScoreManager.casualties * 50).ToString();
        demonsWereAlertedText.text = "Stealth bonus multiplier : " + ((ScoreManager.demonsWereAlerted) ? "x 1" : "x 2");

        score = ScoreManager.lettersCollected * 100 - ScoreManager.casualties * 50;
        if (ScoreManager.demonsWereAlerted)
            score *= 2;

        scoreText.text = "Score : " + score;
    }
}
