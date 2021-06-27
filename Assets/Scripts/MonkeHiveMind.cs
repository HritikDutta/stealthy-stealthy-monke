using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MonkeHiveMind : MonoBehaviour
{
    [Header("Input")]
    public float switchInterval = 1f;

    // [Header("UI")]
    // public Text monkeyCountText;
    // public Text selectedSquadText;

    [Header("Units")]
    public List<MonkeSquad> squads = new List<MonkeSquad>();

    private Camera camera;

    private int selectedSquadIndex = 0;

    private float lastSwitchTime = 0f;

    private int numSquadsLeftInSection;

    [HideInInspector]
    public int numActiveSquads;

    void Awake()
    {
        camera = Camera.main;
    }

    void Start()
    {
        // int monkeCount = 0;
        // for (int i = 0; i < squads.Count; i++)
        //     monkeCount += squads[i].monkes.Count;

        // monkeyCountText.text = monkeCount.ToString();
        // selectedSquadText.text = selectedSquadIndex.ToString();
        Level.mouseOver.SetColor(squads[selectedSquadIndex].color);
    }

    void Update()
    {
        // No one left to control :/
        if (numActiveSquads <= 0)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPosition = Input.mousePosition;
            Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;

            Vector3Int gridPosition = Level.groundTilemap.WorldToCell(worldPosition);

            if (IsInSection(gridPosition) && Level.groundTilemap.HasTile(gridPosition) && !Level.wallTilemap.HasTile(gridPosition))
                squads[selectedSquadIndex].TellMonkesToMoveAsses(gridPosition);
        }

        if (Time.time - lastSwitchTime >= switchInterval)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                selectedSquadIndex = (selectedSquadIndex + 1) % squads.Count;
                while (squads[selectedSquadIndex].Eliminated || squads[selectedSquadIndex].finished)
                    selectedSquadIndex = (selectedSquadIndex + 1) % squads.Count;

                // selectedSquadText.text = selectedSquadIndex.ToString();
                Level.mouseOver.SetColor(squads[selectedSquadIndex].color);
                lastSwitchTime = Time.time;
            }

            if (Input.GetKeyDown(KeyCode.Q) || Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                selectedSquadIndex = (selectedSquadIndex + squads.Count - 1) % squads.Count;
                while (squads[selectedSquadIndex].Eliminated || squads[selectedSquadIndex].finished)
                    selectedSquadIndex = (selectedSquadIndex + squads.Count - 1) % squads.Count;

                // selectedSquadText.text = selectedSquadIndex.ToString();
                Level.mouseOver.SetColor(squads[selectedSquadIndex].color);
                lastSwitchTime = Time.time;
            }
        }
    }

    private bool IsInSection(Vector3Int position)
    {
        return position.y < Level.sectionTop && position.y >= Level.sectionBottom;
    }

    public void Init()
    {
        foreach (MonkeSquad squad in squads)
            squad.Init();
        
        numActiveSquads = numSquadsLeftInSection = squads.Count;
    }

    public void TeleportEveryone(Vector3Int gridPosition)
    {
        foreach (MonkeSquad squad in squads)
            squad.TeleportEveryone(gridPosition);

        // Just to be safe
        Level.mouseOver.SetColor(squads[selectedSquadIndex].color);
    }

    public void SendSquadToPosition(MonkeSquad squad, Vector3Int gridPosition)
    {
        if (squad.Eliminated || squad.finished)
            return;

        squad.finished = true;
        squad.TeleportEveryone(gridPosition);
        numSquadsLeftInSection--;

        if (numSquadsLeftInSection <= 0)
        {
            foreach (MonkeSquad s in squads)
                s.finished = false;
                
            Level.GoToNextSection();
            numSquadsLeftInSection = numActiveSquads;
        }
        else
        {
            selectedSquadIndex = (selectedSquadIndex + 1) % squads.Count;
            while (squads[selectedSquadIndex].Eliminated || squads[selectedSquadIndex].finished)
                selectedSquadIndex = (selectedSquadIndex + 1) % squads.Count;

            Level.mouseOver.SetColor(squads[selectedSquadIndex].color);
        }
    }

    public void DemonStartedChasing(MonkeSquad squad, Transform guardTransform)
    {
        Level.audio.Play("Monkey Panic");
        foreach (MonkeBehaviour monke in squad.monkes)
            monke.StartRunningAway(guardTransform);
    }

    public void DemonStoppedChasing(MonkeSquad squad)
    {
        foreach (MonkeBehaviour monke in squad.monkes)
            monke.StopRuningAway();
    }

    public void Captured(MonkeBehaviour monke)
    {
        monke.GetCaptured();

        if (monke.mySquad.Eliminated)
        {
            numSquadsLeftInSection--;
            numActiveSquads--;
            
            if (numActiveSquads <= 0)
            {
                // @Todo: Failure condition
                Debug.Log("Level Failed");
                Level.mouseOver.Disable();
                return;
            }

            // Change highlight color
            selectedSquadIndex = (selectedSquadIndex + 1) % squads.Count;
            while (squads[selectedSquadIndex].Eliminated || squads[selectedSquadIndex].finished)
                selectedSquadIndex = (selectedSquadIndex + 1) % squads.Count;

            Level.mouseOver.SetColor(squads[selectedSquadIndex].color);

            if (numSquadsLeftInSection <= 0 && numActiveSquads > 0)
                Level.GoToNextSection();
        }

        // int monkeCount = 0;
        // for (int i = 0; i < squads.Count; i++)
        //     monkeCount += squads[i].numActiveMonkes;

        // monkeyCountText.text = monkeCount.ToString();
    }
}
