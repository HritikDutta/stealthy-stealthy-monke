using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeSquad : MonoBehaviour
{
    public Transform commandBanana;
    public List<MonkeBehaviour> monkes = new List<MonkeBehaviour>();
    public int numActiveMonkes = 0;

    void Start()
    {
        foreach (Transform child in transform)
        {
            MonkeBehaviour monke = child.GetComponent<MonkeBehaviour>();
            monke.FindPathToBanana(commandBanana);
            monkes.Add(monke);
            numActiveMonkes++;
        }
    }

    public void TellMonkesToMoveAsses(Vector3Int gridPosition)
    {
        commandBanana.transform.position = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);
        foreach (MonkeBehaviour monke in monkes)
            monke.FindPathToBanana(commandBanana);
    }

    public void TellThemIDied()
    {
        numActiveMonkes--;
    }
}
