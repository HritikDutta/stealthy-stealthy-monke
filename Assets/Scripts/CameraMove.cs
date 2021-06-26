using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveDuration = 10f;
    public AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private float lastMove = -10f;
    private Vector3 start;
    private Vector3 end;

    private Transform mask;
    private Vector3 maskStartScale;
    private Vector3 maskTargetScale;

    void Awake()
    {
        mask = transform.GetChild(0);
    }

    void Update()
    {
        float t = (Time.time - lastMove) / moveDuration;
        if (t > 1f)
            return;
        
        transform.position = Vector3.Lerp(start, end, moveCurve.Evaluate(t));
        mask.localScale = Vector3.Lerp(maskStartScale, maskTargetScale, moveCurve.Evaluate(t));
    }

    public void SetTargetAndMaskScale(Transform _target, Vector3 scale)
    {
        lastMove = Time.time;
        start = transform.position;
        end = _target.position;

        maskTargetScale = scale;
        maskStartScale = mask.localScale;
    }
}
