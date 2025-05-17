using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    // 플레이어 몸체와 카메라에 대한 참조
    [SerializeField]
    private Transform PlayerBody; // 플레이어 오브젝트 (캐릭터 모델)
    [SerializeField]
    private Transform CameraDT; // 카메라의 부모 객체 (카메라의 회전 및 위치 제어)

    private Vector3 cameraOffset; // 카메라와 플레이어 간의 오프셋 (카메라의 위치를 결정)

    void Start()
    {
    }

    void Update()
    {
        LookAround();
        Move();
    }
    // 플레이어의 이동 처리
    private void Move()
    {
        // 이동 방향을 카메라의 회전과 일치하도록 설정 (수평 방향으로만 이동)
        Debug.DrawRay(CameraDT.position, new Vector3(CameraDT.forward.x, 0f, CameraDT.forward.z).normalized, Color.red);
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0; // 입력값이 있는지 확인 (이동이 있는지)

        // 이동 입력이 있으면
        if (isMove)
        {
            // 카메라가 바라보는 방향을 기준으로 이동하도록 설정
            // 수평 방향은 카메라의 오른쪽(직각 방향)으로 설정
            Vector3 looKForwad = new Vector3(CameraDT.forward.x, 0f, CameraDT.forward.z).normalized;
            Vector3 LookRight = new Vector3(CameraDT.right.x, 0f, CameraDT.right.z).normalized;
            // 이동 방향 계산
            Vector3 MoveDir = looKForwad * moveInput.y + LookRight * moveInput.x;
            PlayerBody.forward = looKForwad;
            //계산된 이동 방향에 맞춰 플레이어의 위치 변경
            transform.position += MoveDir * Time.deltaTime * 5f;
        }
    }

    // 카메라 회전 처리
    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = CameraDT.rotation.eulerAngles;
        // 카메라의 수직 회전 범위를 제한 (마우스 Y축 이동)
        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);  // 수직 회전 범위 제한
        }
        else
        {
            x = Mathf.Clamp(x, 340f, 361f); // 수직 회전 범위 제한
        }

        // 카메라의 수평 회전도 마우스 X축 이동에 따라 변하도록 설정
        CameraDT.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
}