using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    public static Level instance;

    [Header("Level")]
    public Tilemap _groundTilemap;
    public Tilemap _wallTilemap;
    public Tilemap _itemTilemap;

    [Header("Demons")]
    public Transform _demonsObject;

    private List<DemonBehaviour> _demons = new List<DemonBehaviour>();
    private PathFinder _finder;

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
