using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @Watch: https://youtu.be/YnwOoxtgZQI
// @Watch: https://learn.unity.com/tutorial/2d-roguelike-setup-and-assets?uv=5.x&projectId=5c514a00edbc2a0020694718#5c7f8528edbc2a002053b6f8

public class PlaceBanana : MonoBehaviour
{
    public Transform bananaTransform;
    public Camera camera;

    private float imageScale;

    void Start()
    {
        camera = Camera.main;
        imageScale = (Screen.height / 193) * 12;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 screenPosition = Input.mousePosition;
            screenPosition.x = ((Mathf.Floor(screenPosition.x / imageScale) + 0.5f) * imageScale);
            screenPosition.y = ((Mathf.Floor(screenPosition.y / imageScale) + 0.5f) * imageScale);

            Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;
            bananaTransform.position = worldPosition;
        }
    }
}
