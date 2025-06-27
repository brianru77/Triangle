using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anime;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 60f;           // 대시 속도
    [SerializeField] private float maxDashDistance = 10f;      // 최대 대시 거리
    [SerializeField] private float stopBeforeDistance = 0.3f; // 목표 지점에서 멈출 거리
    [SerializeField] private LayerMask dashLayerMask;         // 바닥 레이어 (대시 타겟 지정용)

    private bool isDashing = false;     // 대시 중인지 여부
    private bool isAttacking = false;   // 공격 중인지 여부

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
    }

    void Update()
    {
        // 우클릭 대시
        if (Input.GetMouseButtonDown(1) && !isDashing)
        {
            TryDashTowardMouse();
        }

        // 좌클릭 공격
        if (Input.GetMouseButtonDown(0))
        {
            isAttacking = true;
            left_mouse_Attack();
        }

        // 좌클릭 떼면 공격 종료
        if (Input.GetMouseButtonUp(0))
        {
            isAttacking = false;
            anime.SetBool("isAttacking", false); // 공격 상태 해제
        }
    }

    // 공격 애니메이션 실행
    void left_mouse_Attack()
    {
        anime.SetBool("isAttacking", true);
    }

    // 마우스 위치 기준으로 대시 시도
    void TryDashTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 마우스 위치로 레이캐스트하여 바닥 클릭 여부 확인
        if (Physics.Raycast(ray, out hit, 100f, dashLayerMask))
        {
            Vector3 target = hit.point;
            Vector3 dir = (target - transform.position).normalized;
            Vector3 dashTarget = transform.position + dir * maxDashDistance;

            float fullDistance = Vector3.Distance(transform.position, target);
            float dashDistance = Mathf.Min(fullDistance - stopBeforeDistance, maxDashDistance);

            if (dashDistance <= 0.2f) return; // 너무 가까우면 무시

            // 애니메이션 상태 변경 및 속도 증가
            if (anime != null)
            {
                anime.SetBool("isDashing", true);
                anime.speed = 4f;  // 대시 중 애니메이션 속도 증가
            }

            // 실제 대시 이동 시작
            StartCoroutine(DashRoutine(dashTarget));
        }
    }
    // 대시 코루틴 실행
    IEnumerator DashRoutine(Vector3 targetPosition)
    {
        isDashing = true;

        if (anime != null)
        {
            anime.SetBool("isDashing", true);  // 대시 상태 true
        }

        float elapsed = 0f;
        float duration = Vector3.Distance(transform.position, targetPosition) / dashSpeed;
        Vector3 start = transform.position;

        // 부드럽게 목표 지점까지 이동
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(start, targetPosition, elapsed / duration);
            rb.MovePosition(newPos);  // 물리 이동
            yield return null;
        }

        rb.MovePosition(targetPosition); // 마지막 위치 보정

        // 대시 종료 처리
        isDashing = false;
        if (anime != null)
        {
            anime.SetBool("isDashing", false); // 대시 종료
            anime.speed = 1f;                  // 애니메이션 속도 원복
        }
    }
}