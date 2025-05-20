using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Jump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float doubleJumpForce = 10f;
    [SerializeField] private float TripleJumpForce = 5f;
    public int currentJumpCount = 0;
    private Rigidbody rb;
    private Animator anime;
    public bool Double_Jump;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
    }

    void Update()
    {
        HandleJump();
    }
    public void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount == 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // 기존 y축 속도 초기화
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            anime.SetInteger("Jump", currentJumpCount);
            Debug.Log("1단 점프 실행됨");
            Debug.Log(currentJumpCount);
            currentJumpCount = 1;
        }
        if (Input.GetKeyUp(KeyCode.Space) && currentJumpCount == 1)
        {
            Double_Jump = true;
        }
        if (Double_Jump)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // 기존 y축 속도 초기화
                rb.AddForce(Vector3.up * doubleJumpForce, ForceMode.VelocityChange);
                anime.SetInteger("Jump", currentJumpCount);
                Debug.Log("더블점프 실행됨");
                Debug.Log(currentJumpCount);
                currentJumpCount = 2;
            }
        }
        if (currentJumpCount >= 2)
        {
            Double_Jump = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            currentJumpCount = 0;
        }
    }
}