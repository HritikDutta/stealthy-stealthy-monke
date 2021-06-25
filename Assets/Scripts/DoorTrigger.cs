using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public LayerMask mask;

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col != null && ((1 << col.gameObject.layer) & mask) != 0)
        {
            Level.GoToNextSection();
            Debug.Log("Section End!");
        }
    }
}
