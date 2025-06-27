using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [Header("mini Cam")]
    [SerializeField] private Transform player;            // 따라갈 플레이어
    [SerializeField] private float cameraHeight = 160f;    // 위에서 내려다보는 높이

    void LateUpdate()
    {
        UpdateMiniMapCamera();
    }

    private void UpdateMiniMapCamera()
    {
        // 카메라를 플레이어 위로 위치
        Vector3 newPos = player.position;
        newPos.y += cameraHeight;
        transform.position = newPos;

        // 카메라의 Y 회전은 플레이어 방향과 일치
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}