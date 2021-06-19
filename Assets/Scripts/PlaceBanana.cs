using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @Watch: https://youtu.be/YnwOoxtgZQI

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
