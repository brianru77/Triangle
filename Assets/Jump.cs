using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Jump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float doubleJumpForce = 5f;
    public int currentJumpCount = 0;
    private Rigidbody rb;
    private Animator anime;
    public bool Double_Jump;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
        anime.SetInteger("Jump", 0);
    }

    void Update()
    {
        HandleJump();
    }
    public void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount == 0)
        {
            currentJumpCount = 1; // 먼저 바꾸고
            anime.SetInteger("Jump", currentJumpCount); // 그리고 Animator에 알려줌
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            Debug.Log("1단 점프");
        }
        if (Input.GetKeyUp(KeyCode.Space) && currentJumpCount == 1)
        {
            Double_Jump = true;
        }
        if (Double_Jump && Input.GetKeyDown(KeyCode.Space))
        {
            currentJumpCount = 2;
            anime.SetInteger("Jump", currentJumpCount);
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * doubleJumpForce, ForceMode.VelocityChange);
            Double_Jump = false; // 안전하게 꺼줌
            Debug.Log("2단 점프");
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
            anime.SetInteger("Jump", 0); // 초기화
             Debug.Log("땅에 닿음");
        }
    }
}