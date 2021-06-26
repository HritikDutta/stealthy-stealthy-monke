using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeSquad : MonoBehaviour
{
    public Transform commandBanana;
    public Color color;
    
    public List<MonkeBehaviour> monkes = new List<MonkeBehaviour>();
    public int numActiveMonkes = 0;

    private SpriteRenderer bananaRenderer;

    void Awake()
    {
        bananaRenderer = commandBanana.GetComponent<SpriteRenderer>();

        foreach (Transform child in transform)
        {
            MonkeBehaviour monke = child.GetComponent<MonkeBehaviour>();
            monkes.Add(monke);
            numActiveMonkes++;
        }
    }

    void Start()
    {
        foreach (MonkeBehaviour monke in monkes)
            monke.FindPathToBanana(commandBanana);
    }

    void Update()
    {
        if (bananaRenderer.enabled)
        {
            bool everyoneHasReached = true;
            foreach (MonkeBehaviour monke in monkes)
            {
                if (monke.mood == MonkeMood.BananaYumYum)
                {
                    everyoneHasReached = false;
                    break;
                }
            }

            bananaRenderer.enabled = !everyoneHasReached;
        }
    }

    public void TeleportEveryone(Vector3Int gridPosition)
    {
        Vector3 floatPosition = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);
        commandBanana.transform.position = floatPosition;
        bananaRenderer.enabled = false;

        foreach (MonkeBehaviour monke in monkes)
        {
            Vector3Int dest = monke.FindPositionAroundBanana(gridPosition);
            Vector3 floatDest = (Vector3) dest + new Vector3(0.5f, 0.5f, 0f);

            monke.Teleport(floatDest);
        }

        numActiveMonkes = monkes.Count;
    }

    public void TellMonkesToMoveAsses(Vector3Int gridPosition)
    {
        commandBanana.transform.position = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);
        bananaRenderer.enabled = true;

        foreach (MonkeBehaviour monke in monkes)
            monke.FindPathToBanana(commandBanana);
    }

    public void TellThemIDied()
    {
        numActiveMonkes--;
    }
}
