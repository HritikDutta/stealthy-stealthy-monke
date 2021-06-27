using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LettersLeftDisplay : MonoBehaviour
{
    private Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = Level.lettersCollected.ToString() + " / " + Level.lettersInLevel;
    }
}
