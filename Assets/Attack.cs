using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anime;

    [Header("Dash Settings")]
    private float dashSpeed = 60f;
    private float maxDashDistance = 12f;
    private float punchDistance = 0.1f;
    private float targetSearchRadius = 10f;
    [SerializeField] private LayerMask dashTargetMask;

    [Header("Attack Settings")]
    [SerializeField] private float attackDuration = 0.8f;

    private bool isDashing = false;
    public bool isAttacking = false;
    public bool isSnaping = false;

    private GameObject dashTargetObject = null;

    [Header("Snap Effect Settings")]
    public GameObject Snap_Effects;
    public float impactSpeed = 30f;
    public Transform firePoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
    }

    void Update()
    {
        //DebugRaycast();
        DebugFirePointAndRay();
        //좌클릭+우클릭 동시 (Snap 우선)
        if (Input.GetMouseButtonDown(2))
        {
            StartCoroutine(SnapRoutine());
            return;
        }

        //일반 공격 (Snap 중이 아닐 때만)
        if (Input.GetMouseButtonDown(0) && !isSnaping)
        {
            StartCoroutine(AttackRoutine());
        }

        //스마트 대시 (Snap 중 또는 대시 중 아닐 때만)
        if (Input.GetMouseButtonDown(1) && !isDashing && !isSnaping)
        {
            TrySmartDash();
        }
    }
    void DebugFirePointAndRay()
    {
        if (firePoint == null || Camera.main == null) return;

        //firePoint 위치 출력
        Debug.Log("[🔵 firePoint.position] " + firePoint.position.ToString("F4"));

        //카메라 기준 레이 생성 (마우스 위치 기준)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red); //씬에 빨간 선으로 시각화

        //레이 정보 출력
        Debug.Log("[🔴 Ray Direction] " + ray.direction.ToString("F4"));
    }
    void DebugRaycast() //레이 제대로 확인
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green, 2f);  //명확한 선 (성공)
            Debug.Log("Ray hit: " + hit.collider.name);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f); //안 맞아도 그리기
            Debug.Log("Ray missed");
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        anime.SetBool("isAttacking", true);
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
        anime.SetBool("isAttacking", false);
    }

    IEnumerator SnapRoutine()
    {
        isSnaping = true;
        anime.SetBool("isSnaping", true);

        snap(); //발사 이펙트 처리

        yield return new WaitForSeconds(0.6f); //애니메이션 길이만큼 대기 (조정 가능)

        isSnaping = false;
        anime.SetBool("isSnaping", false);
    }

    void snap()
    {
        if (Snap_Effects == null || firePoint == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Raycast로 충돌지점 찾기
        Vector3 direction;
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            direction = (hit.point - firePoint.position).normalized;
            Debug.DrawLine(firePoint.position, hit.point, Color.green, 2f);
        }
        else
        {
            //충돌이 없을 때: 마우스 방향을 기반으로 먼 위치를 임의로 잡아서 쏨
            Vector3 fallbackPoint = ray.GetPoint(100f);
            direction = (fallbackPoint - firePoint.position).normalized;
            Debug.DrawLine(firePoint.position, fallbackPoint, Color.yellow, 2f);
        }

        //방향이 너무 짧거나 0에 가까우면 보정
        if (direction == Vector3.zero)
            direction = firePoint.forward;

        //이펙트 생성 및 발사
        GameObject impact = Instantiate(Snap_Effects, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = impact.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * impactSpeed;
        }

        Destroy(impact, 2f); //2초 후 자동 삭제
    }

    void TrySmartDash()
    {
        GameObject nearest = FindClosestTargetInRange();
        Vector3 target;
        Vector3 dir;

        if (nearest != null)
        {
            dir = (nearest.transform.position - transform.position).normalized;
            target = nearest.transform.position - dir * punchDistance;
            dashTargetObject = nearest;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, 100f, dashTargetMask)) return;

            dir = (hit.point - transform.position).normalized;
            target = hit.point;
            dashTargetObject = null;
        }

        float distance = Vector3.Distance(transform.position, target);
        float dashDistance = Mathf.Min(distance, maxDashDistance);

        if (dashDistance <= 0.2f) return;

        Vector3 dashTarget = transform.position + dir * dashDistance;

        if (anime != null)
            anime.SetBool("isDashing", true);

        StartCoroutine(DashRoutine(dashTarget));
    }

    IEnumerator DashRoutine(Vector3 targetPosition)
    {
        isDashing = true;

        float elapsed = 0f;
        float duration = Vector3.Distance(transform.position, targetPosition) / dashSpeed;
        Vector3 start = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(start, targetPosition, elapsed / duration);
            rb.MovePosition(newPos);

            if (dashTargetObject != null)
            {
                if (Vector3.Distance(newPos, dashTargetObject.transform.position) <= punchDistance)
                {
                    Vector3 stopDir = (dashTargetObject.transform.position - transform.position).normalized;
                    rb.MovePosition(dashTargetObject.transform.position - stopDir * punchDistance);
                    dashTargetObject = null;
                    break;
                }
            }

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

    GameObject FindClosestTargetInRange()
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag("Mos");
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject target in allTargets)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < targetSearchRadius && dist < minDist)
            {
                closest = target;
                minDist = dist;
            }
        }

        return closest;
    }
}