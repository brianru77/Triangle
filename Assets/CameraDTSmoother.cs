using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDTSmoother : MonoBehaviour
{
    public Transform target; // 보통 Player의 머리 위 위치
    public Vector3 offset = new Vector3(0f, 2f, 0f); // 기준 오프셋
    public float smoothTime = 0.05f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
    }
}
