using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // 따라갈 대상 (플레어)
    public float distance = 5f;  // 카메라와 대상 사이의 거리
    public float height = 3f;  // 카메라의 높이
    public float rotationSpeed = 5f;  // 카메라 회전 속도

    private void Update()
    {
        if (target != null)
        {
            // 마우스 입력을 받아 카메라 회전
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
            transform.RotateAround(target.position, Vector3.up, horizontal);

            // 카메라의 위치를 계산하여 따라다니게 함
            Vector3 targetPosition = target.position - transform.forward * distance + Vector3.up * height;
            transform.position = targetPosition;

            // 카메라는 항상 플레어를 바라봄
            transform.LookAt(target);
        }
    }
}