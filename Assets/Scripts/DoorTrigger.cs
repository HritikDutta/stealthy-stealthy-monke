using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorType
{
    SectionEnd,
    LevelEnd
}

public class DoorTrigger : MonoBehaviour
{
    public DoorType type;
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
            MonkeBehaviour monke = col.transform.GetComponent<MonkeBehaviour>();

            switch (type)
            {
                case DoorType.SectionEnd:
                {
                    Level.SendSquadToNextSection(monke.mySquad);
                } break;

                case DoorType.LevelEnd:
                {
                    Level.UnlockDoor(monke.mySquad);

                    if (Level.doorIsOpen)
                        Level.SendSquadToNextSection(monke.mySquad);
                } break;
            }
        }
    }
}
