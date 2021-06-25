using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    public static Level instance;

    [Header("Level")]
    public CameraMove _levelCamera;
    public Tilemap _groundTilemap;
    public Tilemap _wallTilemap;
    public Tilemap _itemTilemap;

    [Header("Monkes")]
    public MonkeHiveMind _hiveMind;

    [Header("Demons")]
    public Transform _demonsObject;

    private List<DemonBehaviour> _demons = new List<DemonBehaviour>();
    private PathFinder _finder;

    private List<Transform> _cameraPositions = new List<Transform>();
    private List<Transform> _sectionStarts = new List<Transform>();
    private List<DoorTrigger> _sectionTriggers = new List<DoorTrigger>();
    private int _currentTriggerIndex = 0;

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
        _sectionTriggers[_currentTriggerIndex].Enable();
        _levelCamera.target = _cameraPositions[_currentTriggerIndex];
        _hiveMind.TeleportEveryone(_groundTilemap.WorldToCell(_sectionStarts[_currentTriggerIndex].position));
    }

    public static void GoToNextSection()
    {
        if (instance._currentTriggerIndex >= instance._sectionTriggers.Count)
            return;

        instance._sectionTriggers[instance._currentTriggerIndex].Disable();
        instance._currentTriggerIndex++;
        
        if (instance._currentTriggerIndex >= instance._sectionTriggers.Count)
            return;

        instance._sectionTriggers[instance._currentTriggerIndex].Enable();
        instance._levelCamera.target = instance._cameraPositions[instance._currentTriggerIndex];
        instance._hiveMind.TeleportEveryone(instance._groundTilemap.WorldToCell(instance._sectionStarts[instance._currentTriggerIndex].position));
    }

    public static Tilemap groundTilemap {
        get { return instance._groundTilemap; }
    }

    public static Tilemap wallTilemap {
        get { return instance._wallTilemap; }
    }

    public static Tilemap itemTilemap {
        get { return instance._itemTilemap; }
    }

    public static List<DemonBehaviour> demons {
        get { return instance._demons; }
    }

    public static PathFinder finder {
        get { return instance._finder; }
    }
}
