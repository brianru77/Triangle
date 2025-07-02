using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Jump : MonoBehaviour
{
    public float jumpForce = 15f;
    public float doubleJumpForce = 25f;
    public int currentJumpCount = 0;
    private Rigidbody rb;
    private Animator anime;
    public bool Double_Jump;
    public bool Triple_Jump;
    public float YVelocity;
    public GameObject Jump_shockwave_effects; //충격파 이펙트
    public float Jump_shockwave_Radius = 50f; //충격파 범위
    public int Jump_shockwave_Damage = 30;   //충격파 데미지
    private bool is_Jump_shockwave = false;
    //중력 강화_낙하 속도 빠르게
    [SerializeField] private float fallMultiplier = 1.15f; //인게임에서 리셋해줘야함

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
    void Jump_CreateShockwave()
    {
        Instantiate(Jump_shockwave_effects, transform.position, Quaternion.identity);
        Destroy(Jump_shockwave_effects, 2f); // 2초 후 자동 삭제
        Debug.Log("착지 충격파 발생!");
    }
    public void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount == 0)
        {
            currentJumpCount = 1; //먼저 바꾸고
            anime.SetInteger("Jump", currentJumpCount); //그리고 Animator에 알려줌
            //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            Debug.Log("1단 점프");
            //rb.AddForce(transform.forward * 3f, ForceMode.VelocityChange);
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
            Double_Jump = false; //안전하게 꺼줌
            Debug.Log("2단 점프");
            //rb.AddForce(transform.forward * 5f, ForceMode.VelocityChange);
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

            rb.AddForce(Vector3.down * 20, ForceMode.VelocityChange);
            //rb.AddForce(transform.forward * 25f, ForceMode.VelocityChange);
            anime.speed = 1.2f;
            is_Jump_shockwave = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            currentJumpCount = 0;
            anime.SetInteger("Jump", 0); //초기화
            Debug.Log("땅에 닿음");
            //점프 상태 초기화
            Double_Jump = false;
            Triple_Jump = false;

            if (is_Jump_shockwave)
            {
                Invoke("Jump_CreateShockwave", 0.2f); // 충격파 발생
                is_Jump_shockwave = false; //발생 후 초기화
            }

            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //리지드바디 잔상 제거
            if (flatVelocity.magnitude > 0.1f)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0); //XZ 이동속도 제거
            }
        }
    }
}