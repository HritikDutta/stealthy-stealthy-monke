using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private int _lettersCollected;
    private int _totalLetters;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    public static void Reset()
    {
        instance._lettersCollected = 0;
        instance._totalLetters = 0;
    }

    public static void AddLetterCounts(int total, int collected)
    {
        instance._lettersCollected += collected;
        instance._totalLetters += total;
    }

    public static int lettersCollected {
        get { return instance._lettersCollected; }
    }

    public static int totalLetters {
        get { return instance._totalLetters; }
    }

}
