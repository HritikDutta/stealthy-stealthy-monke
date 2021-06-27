using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    public string mainMenuSceneName;

    public void GoToMainMenu()
    {
        Application.LoadLevel(mainMenuSceneName);
    }
}
