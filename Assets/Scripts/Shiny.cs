using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shiny : MonoBehaviour
{
    public bool broken = false;
    
    public void Break()
    {
        if (broken)
            return;

        Debug.Log("Oh no it broke!");
        gameObject.SetActive(false);
        broken = true;
    }
}
