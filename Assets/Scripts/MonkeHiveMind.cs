using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MonkeHiveMind : MonoBehaviour
{
    public static MonkeHiveMind instance;

    [Header("Input")]
    public float switchInterval = 1f;

    [Header("UI")]
    public Text monkeyCountText;
    public Text selectedSquadText;

    [Header("Units")]
    public List<MonkeSquad> squads = new List<MonkeSquad>();

    private Camera camera;

    private int selectedSquadIndex = 0;

    [HideInInspector]
    public bool collectedKey;

    private float lastSwitchTime = 0f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        camera = Camera.main;
    }

    void Start()
    {
        int monkeCount = 0;
        for (int i = 0; i < squads.Count; i++)
            monkeCount += squads[i].monkes.Count;

        monkeyCountText.text = monkeCount.ToString();
        selectedSquadText.text = selectedSquadIndex.ToString();
        Level.mouseOver.SetColor(squads[selectedSquadIndex].color);
        
        collectedKey = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPosition = Input.mousePosition;
            Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;

            Vector3Int gridPosition = Level.groundTilemap.WorldToCell(worldPosition);

            if (Level.groundTilemap.HasTile(gridPosition) && !Level.wallTilemap.HasTile(gridPosition))
                squads[selectedSquadIndex].TellMonkesToMoveAsses(gridPosition);
        }

        if (Time.time - lastSwitchTime >= switchInterval)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                selectedSquadIndex = (selectedSquadIndex + 1) % squads.Count;
                selectedSquadText.text = selectedSquadIndex.ToString();
                Level.mouseOver.SetColor(squads[selectedSquadIndex].color);
                lastSwitchTime = Time.time;
            }

            if (Input.GetKeyDown(KeyCode.Q) || Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                selectedSquadIndex = (selectedSquadIndex + squads.Count - 1) % squads.Count;
                selectedSquadText.text = selectedSquadIndex.ToString();
                Level.mouseOver.SetColor(squads[selectedSquadIndex].color);
                lastSwitchTime = Time.time;
            }
        }
    }

    public void RegisterKey()
    {
        collectedKey = true;
    }

    public void TeleportEveryone(Vector3Int gridPosition)
    {
        foreach (MonkeSquad squad in squads)
            squad.TeleportEveryone(gridPosition);
    }

    public void DemonStartedChasing(MonkeSquad squad, Transform guardTransform)
    {
        foreach (MonkeBehaviour monke in squad.monkes)
            monke.StartRuningAway(guardTransform);
    }

    public void DemonStoppedChasing(MonkeSquad squad)
    {
        foreach (MonkeBehaviour monke in squad.monkes)
            monke.StopRuningAway();
    }

    public void Captured(MonkeBehaviour monke)
    {
        monke.GetCaptured();

        int monkeCount = 0;
        for (int i = 0; i < squads.Count; i++)
            monkeCount += squads[i].numActiveMonkes;

        monkeyCountText.text = monkeCount.ToString();
    }
}
