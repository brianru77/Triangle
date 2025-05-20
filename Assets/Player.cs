using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform PlayerBody;  // 캐릭터 외형 (자식 오브젝트)
    [SerializeField] private Transform CameraDT;    // 카메라 회전 기준점 (카메라의 부모)

    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float moveSpeed = 5f;

    private float xRotation = 0f; // 상하 회전 누적값
    private float yRotation = 0f; // 좌우 회전 누적값
    private Animator anime;
    Rigidbody rb; //transform.position을 직접 조작하니까 물리를 무시하고 이동함
    private bool Flying;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Flying = false;
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        anime.applyRootMotion = false; //
    }

    void Update()
    {
        LookAround();
    }
    void FixedUpdate()
    {
        //Debug.Log($"RB Pos: {rb.position}, Velocity: {rb.velocity}"); //다른 힘이 적용받는지 체크용
        Move();
        Fly();
    }
    void Fly()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Flying = true;
            rb.AddForce(Vector3.up * 30.0f, ForceMode.Acceleration); //상승값
            Vector3 dir2 = new Vector3(0, 0, 0);
        }
        if (Flying)
        {
            anime.enabled = false;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Flying = false;
            anime.enabled = true;
        }
    }
    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;

        yRotation += mouseDelta.x;
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -25f, 70f); // 위아래 회전 제한

        // ▶ 카메라는 상하 회전만
        CameraDT.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ▶ 몸체는 좌우 회전 (Player 자체를 회전시킴)
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMoving = moveInput.magnitude > 0f;
        anime.SetBool("Walk", isMoving);

        // 기본 이동 속도 설정
        float currentSpeed = moveSpeed;

        // 달리기 조건: 이동 중 + Shift
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);
        if (isRunning)
        {
            currentSpeed = 20f;
        }
        anime.SetBool("Run", isRunning);
        //불셋 애니는 애니메이터 변수이름 int는 자체 애니메이션 이름
        bool isAcceleration = Input.GetKey(KeyCode.Space);
        if (isAcceleration)
        {
            anime.speed = 5f;
            transform.position += transform.forward * 3f;
            anime.SetBool("Accel", isMoving);
        }
        else
        {
            anime.speed = 1f;
            anime.SetBool("Accel", false);
        }

        if (isMoving)
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            // 수평 방향만 유지
            Vector3 moveDir = forward * moveInput.y + right * moveInput.x;
            moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up);

            //현재 Y는 유지하고, XZ만 MovePosition으로 이동
            Vector3 targetPos = rb.position + moveDir.normalized * currentSpeed * Time.deltaTime;
            targetPos.y = rb.position.y;

            rb.MovePosition(targetPos);
        }
    }
}
