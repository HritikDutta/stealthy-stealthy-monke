using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveDuration = 10f;
    public AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private float lastMove = -10f;
    [HideInInspector] public Transform target;
    private Vector3 start;

    private Transform mask;
    [HideInInspector] public Vector3 maskTargetScale;
    private Vector3 maskStartScale;

    void Awake()
    {
        mask = transform.GetChild(0);
    }

    void Update()
    {
        float t = (Time.time - lastMove) / moveDuration;
        if (t > 1f)
            return;
        
        transform.position = Vector3.Lerp(start, target.position, moveCurve.Evaluate(t));
        mask.localScale = Vector3.Lerp(maskStartScale, maskTargetScale, moveCurve.Evaluate(t));
    }

    public void SetTargetAndMaskScale(Transform _target, Vector3 scale)
    {
        lastMove = Time.time;
        start = transform.position;
        target = _target;

        maskTargetScale = scale;
        maskStartScale = mask.localScale;
    }
}
