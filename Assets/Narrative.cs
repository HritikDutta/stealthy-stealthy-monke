using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Narrative : MonoBehaviour
{
    public GameObject[] images;
    private int index = 0;

    void Start()
    {
        images[0].SetActive(true);
        index = 0;
    }

    public void Next()
    {
        index++;
        if (index >= images.Length)
        {
            Application.LoadLevel(ScoreManager.nextLevel);
            return;
        }

        images[index].SetActive(true);
    }
}
