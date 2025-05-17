using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    [SerializeField] private Transform PlayerBody;  // 캐릭터 외형 (자식 오브젝트)
    [SerializeField] private Transform CameraDT;    // 카메라 회전 기준점 (카메라의 부모)

    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float moveSpeed = 5f;

    private float xRotation = 0f; // 상하 회전 누적값
    private float yRotation = 0f; // 좌우 회전 누적값

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        LookAround();
        Move();
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;

        yRotation += mouseDelta.x;
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -60f, 70f); // 위아래 회전 제한

        // ▶ 카메라는 상하 회전만
        CameraDT.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ▶ 몸체는 좌우 회전 (Player 자체를 회전시킴)
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMoving = moveInput.magnitude > 0f;

        if (isMoving)
        {
            // 캐릭터가 바라보는 방향을 기준으로 이동
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            Vector3 moveDir = forward * moveInput.y + right * moveInput.x;
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
        }
    }
}