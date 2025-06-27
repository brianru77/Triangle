using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Jump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float doubleJumpForce = 15f;
    public int currentJumpCount = 0;
    private Rigidbody rb;
    private Animator anime;
    public bool Double_Jump;
    public bool Triple_Jump;
    public float YVelocity;
    //중력 강화_낙하 속도 빠르게
    [SerializeField] private float fallMultiplier = 1.5f;

    void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Physics.gravity * fallMultiplier, ForceMode.Acceleration); //기본1배+1.5 = 2.5배
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
        anime.SetInteger("Jump", 0);
    }

    void Update()
    {
        HandleJump();
        YVelocity = rb.velocity.y;
        anime.SetFloat("YVelocity", rb.velocity.y); //프레임 낙하상태 감지
        anime.SetInteger("JumpStage", currentJumpCount); //점프 카운트를 애니메이터 변수인 JumpStage로 전달
    }
    public void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount == 0)
        {
            currentJumpCount = 1; // 먼저 바꾸고
            anime.SetInteger("Jump", currentJumpCount); // 그리고 Animator에 알려줌
            //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            Debug.Log("1단 점프");
            rb.AddForce(transform.forward * 3f, ForceMode.VelocityChange);
        }
        if (Input.GetKeyUp(KeyCode.Space) && currentJumpCount == 1)
        {
            Double_Jump = true;
        }
        if (Double_Jump && Input.GetKeyDown(KeyCode.Space) && currentJumpCount == 1)
        {
            currentJumpCount = 2;
            anime.SetInteger("Jump", currentJumpCount);
            rb.AddForce(Vector3.up * doubleJumpForce, ForceMode.VelocityChange);
            Double_Jump = false; // 안전하게 꺼줌
            Debug.Log("2단 점프");
            rb.AddForce(transform.forward * 5f, ForceMode.VelocityChange);
            rb.AddForce(Vector3.down * 3f, ForceMode.VelocityChange);
        }
        if (Input.GetKeyUp(KeyCode.Space) && currentJumpCount == 2)
        {
            Triple_Jump = true;
        }

        if (Triple_Jump && Input.GetKeyDown(KeyCode.Space) && currentJumpCount == 2)
        {
            currentJumpCount = 3;
            anime.SetInteger("Jump", currentJumpCount);
            Triple_Jump = false;
            Debug.Log("3단 착지");

            rb.AddForce(Vector3.down * 30, ForceMode.VelocityChange);
            rb.AddForce(transform.forward * 25f, ForceMode.VelocityChange);
            anime.speed = 1.2f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            currentJumpCount = 0;
            anime.SetInteger("Jump", 0); // 초기화
            Debug.Log("땅에 닿음");
            //점프 상태 초기화
            Double_Jump = false;
            Triple_Jump = false;

            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // 리지드바디 잔상 제거
            if (flatVelocity.magnitude > 0.1f)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0); // XZ 이동속도 제거
            }
        }
    }
}