using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    public static Level instance;

    public Tilemap _groundTilemap;
    public Tilemap _wallTilemap;
    public Tilemap _itemTilemap;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);  // @Todo: Maybe not?
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
}
