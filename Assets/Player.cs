using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        // 입력 받기
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S

        // 방향 계산
        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;

        // 이동
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
    }
}
