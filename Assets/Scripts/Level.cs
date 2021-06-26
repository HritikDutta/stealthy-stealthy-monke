using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    public static Level instance;

    [Header("UI")]
    public MouseOver _mouseOver;

    [Header("Level")]
    public CameraMove _levelCamera;
    public Tilemap _groundTilemap;
    public Tilemap _wallTilemap;
    public Tilemap _interactableTilemap;

    [Header("Monkes")]
    public MonkeHiveMind _hiveMind;

    [Header("Demons")]
    public Transform _demonsObject;

    [Header("Sections")]
    public List<float> _sectionHeights = new List<float>();

    private List<DemonBehaviour> _demons = new List<DemonBehaviour>();
    private PathFinder _finder;

    private List<Transform> _cameraPositions = new List<Transform>();
    private List<Transform> _sectionStarts = new List<Transform>();
    private List<DoorTrigger> _sectionTriggers = new List<DoorTrigger>();
    private int _currentSectionIndex = 0;

    private List<LevelEndDoor> _levelEndDoorTiles = new List<LevelEndDoor>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);  // @Todo: Maybe not?

        foreach (Transform demon in _demonsObject)
            _demons.Add(demon.GetComponent<DemonBehaviour>());

        _finder = GetComponent<PathFinder>();

        Transform triggers = transform.GetChild(0);
        foreach(Transform trigger in triggers)
        {
            DoorTrigger doorTrigger = trigger.GetComponent<DoorTrigger>();
            _sectionTriggers.Add(doorTrigger);
            doorTrigger.Disable();
        }

        Transform _cameraPositionsObject = transform.GetChild(1);
        foreach(Transform cameraPosition in _cameraPositionsObject)
            _cameraPositions.Add(cameraPosition);

        Transform _sectionStartsObject = transform.GetChild(2);
        foreach(Transform sectionStart in _sectionStartsObject)
            _sectionStarts.Add(sectionStart);
    }

    void Start()
    {
        _sectionTriggers[_currentSectionIndex].Enable();
        _hiveMind.TeleportEveryone(_groundTilemap.WorldToCell(_sectionStarts[_currentSectionIndex].position));

        float maskX = (float) (_groundTilemap.size.x + 1);
        Vector3 maskScale =  new Vector3(maskX, _sectionHeights[_currentSectionIndex], 1f);
        _levelCamera.SetTargetAndMaskScale(_cameraPositions[_currentSectionIndex], maskScale);
    }

    public static void AddDoorTile(LevelEndDoor door)
    {
        instance._levelEndDoorTiles.Add(door);
    }

    public static void GoToNextSection()
    {
        if (instance._currentSectionIndex >= instance._sectionTriggers.Count)
            return;

        instance._sectionTriggers[instance._currentSectionIndex].Disable();
        instance._currentSectionIndex++;
        
        if (instance._currentSectionIndex >= instance._sectionTriggers.Count)
            return;

        instance._sectionTriggers[instance._currentSectionIndex].Enable();
        instance._hiveMind.TeleportEveryone(instance._groundTilemap.WorldToCell(instance._sectionStarts[instance._currentSectionIndex].position));

        float maskX = (float) (instance._groundTilemap.size.x + 1);
        Vector3 maskScale =  new Vector3(maskX, instance._sectionHeights[instance._currentSectionIndex], 1f);
        instance._levelCamera.SetTargetAndMaskScale(instance._cameraPositions[instance._currentSectionIndex], maskScale);
    }

    public static void UnlockDoor()
    {
        foreach(LevelEndDoor door in instance._levelEndDoorTiles)
            door.Open(instance._hiveMind.collectedKey);
    }

    public static Tilemap groundTilemap {
        get { return instance._groundTilemap; }
    }

    public static Tilemap wallTilemap {
        get { return instance._wallTilemap; }
    }

    public static Tilemap interactableTilemap {
        get { return instance._interactableTilemap; }
    }

    public static MonkeHiveMind hiveMind {
        get { return instance._hiveMind; }
    }

    public static MouseOver mouseOver {
        get { return instance._mouseOver; }
    }

    public static List<DemonBehaviour> demons {
        get { return instance._demons; }
    }

    public static PathFinder finder {
        get { return instance._finder; }
    }
}
