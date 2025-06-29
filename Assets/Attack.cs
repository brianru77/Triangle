using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anime;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 60f;           //대시 속도
    [SerializeField] private float maxDashDistance = 10f;     //최대 대시 거리
    [SerializeField] private float stopBeforeDistance = 0.3f; //목표 지점에서 멈출 거리
    [SerializeField] private LayerMask dashLayerMask;         //바닥 레이어

    [Header("Attack Settings")]
    [SerializeField] private float attackDuration = 0.8f;     //공격 지속 시간

    private bool isDashing = false;     //대시 중 여부
    public bool isAttacking = false;   //공격 중 여부

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isDashing)
        {
            TryDashTowardMouse();
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(AttackRoutine());
        }
    }

    //공격 처리 코루틴 (애니메이션 자동 해제 포함)
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        anime.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackDuration); //공격 시간만큼 대기

        isAttacking = false;
        anime.SetBool("isAttacking", false);
    }

    //마우스 위치를 기준으로 대시 시도
    void TryDashTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, dashLayerMask))
        {
            Vector3 target = hit.point;
            Vector3 dir = (target - transform.position).normalized;
            float fullDistance = Vector3.Distance(transform.position, target);
            float dashDistance = Mathf.Min(fullDistance - stopBeforeDistance, maxDashDistance);

            if (dashDistance <= 0.2f) return; //너무 가까우면 무시

            Vector3 dashTarget = transform.position + dir * dashDistance;

            if (anime != null)
            {
                anime.SetBool("isDashing", true);
            }

            StartCoroutine(DashRoutine(dashTarget));
        }
    }

    //대시 실행 코루틴
    IEnumerator DashRoutine(Vector3 targetPosition)
    {
        isDashing = true;

        if (anime != null)
        {
            anime.SetBool("isDashing", true);
        }

        float elapsed = 0f;
        float duration = Vector3.Distance(transform.position, targetPosition) / dashSpeed;
        Vector3 start = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(start, targetPosition, elapsed / duration);
            rb.MovePosition(newPos);
            yield return null;
        }

        rb.MovePosition(targetPosition);

        isDashing = false;

        if (anime != null)
        {
            anime.SetBool("isDashing", false);
            anime.speed = 1f;
        }
    }
}