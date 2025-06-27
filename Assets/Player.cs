using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform PlayerBody;  // 캐릭터 외형
    [SerializeField] private Transform CameraDT;    // 카메라 회전 기준점
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float moveSpeed = 5f;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private Animator anime;
    private Rigidbody rb;
    private bool Flying;
    public bool isMoving;
    private Attack get_attackScript; //공격 중 멈추기
    [SerializeField] private Transform respawnTransform;

    void OnCollisionEnter(Collision collision) //충돌_Trigger가 체크 꺼짐
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Flying = false;
            anime.enabled = true; // 땅에 닿으면 애니메이션 다시 활성화
        }
    }
    void OnTriggerEnter(Collider other) //통과_Trigger가 체크 켜짐
    {
        if (other.CompareTag("RespawnZone"))
        {
            transform.position = respawnTransform.position;
            rb.velocity = Vector3.zero; // 속도 초기화
            Debug.Log("리스폰!");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        anime.applyRootMotion = false;
        get_attackScript = GetComponent<Attack>();
    }

    void Update()
    {
        LookAround();

    }

    void FixedUpdate()
    {
        if (get_attackScript != null && get_attackScript.isAttacking)
        {
            rb.velocity = Vector3.zero;
            return; // 공격 중이면 이동 차단
        }
        Move();
        Fly();
    }

    void Fly()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Flying = true;
            rb.AddForce(Vector3.up * 30.0f, ForceMode.Acceleration);
        }

        // 비행 중에는 애니메이션 중지, 땅에 닿으면 다시 활성화됨
        if (!Flying && !anime.enabled)
            anime.enabled = true;
        else if (Flying)
            anime.enabled = false;
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;

        yRotation += mouseDelta.x;
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -25f, 70f);

        CameraDT.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        this.isMoving = moveInput.magnitude > 0.1f;

        float currentSpeed = moveSpeed;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);
        if (moveInput.magnitude > 0.1)
        {
            anime.speed = 1.4f;
        }
        // 달리기 시 속도 증가
        if (isRunning)
            currentSpeed = 20f;

        // 이동 방향 보정
        float moveX = moveInput.x;
        float moveY = moveInput.y;

        // MoveY (앞/뒤) 보정
        if (moveY > 0)
            moveY = isRunning ? 1f : 0.5f;
        else if (moveY < 0)
            moveY = -0.5f;
        else
            moveY = 0f;

        // MoveX (좌/우) 보정
        if (Mathf.Abs(moveX) > 0)
            moveX = Mathf.Sign(moveX) * 0.5f;
        else
            moveX = 0f;

        // 애니메이터 파라미터 적용
        if (isMoving)
        {
            anime.SetFloat("MoveX", moveX);
            anime.SetFloat("MoveY", moveY);
        }
        else
        {
            anime.SetFloat("MoveX", 0f);
            anime.SetFloat("MoveY", 0f);
        }

        // 실제 위치 이동 처리
        if (isMoving)
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            Vector3 moveDir = forward * moveInput.y + right * moveInput.x;
            moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up); // 수평 이동만 적용

            Vector3 targetPos = rb.position + moveDir.normalized * currentSpeed * Time.deltaTime;
            targetPos.y = rb.position.y; // y 위치 고정

            rb.MovePosition(targetPos); // 이동 적용
        }
    }
}