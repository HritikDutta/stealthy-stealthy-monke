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

    void Update()
    {
        float t = (Time.time - lastMove) / moveDuration;
        if (t > 1f)
            return;
        
        transform.position = Vector3.Lerp(start, end, moveCurve.Evaluate(t));
    }

    public void SetTarget(Transform _target)
    {
        lastMove = Time.time;
        start = transform.position;
        end = _target.position;
    }
}
