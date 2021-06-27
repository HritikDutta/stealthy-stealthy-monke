using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public LayerMask mask;
    private bool hasTriggered;

    public void Enable()
    {
        gameObject.SetActive(true);
        hasTriggered = false;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        hasTriggered = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (hasTriggered)
            return;

        if (col != null && ((1 << col.gameObject.layer) & mask) != 0)
        {
            TutorialManager.TriggerNext();
            hasTriggered = true;
        }
    }
}
