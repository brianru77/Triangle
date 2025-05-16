using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//플레이어 객체에 할당
public class Player : MonoBehaviour
{
    [SerializeField]
    private Transform PlayerBody; //플레이어 오브젝트
    [SerializeField]
    private Transform CamBody; //캠의 부모객체

    public float moveSpeed = 10f;   // 이동 속도 변수 추가
    public float lookSpeed = 8f;   // 카메라 회전 속도 변수 추가

    void Start()
    {
    }
  void Update() //이동
    {
        LookAround();
        Move();
    }
    // Update is called once per frame
    private void Move()
    {
        Debug.DrawRay(CamBody.position, new Vector3(CamBody.forward.x, 0f, CamBody.forward.z).normalized, Color.red);
        //캐릭터 시선을 고정시키는 방법이 없을까 고민해서 만들어봄
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //무브 인풋이 0이면 이동이 없는 것 아니면 있는 것
        bool isMove = moveInput.magnitude != 0;

        if (isMove)
        {
            Vector3 looKForwad = new Vector3(CamBody.forward.x, 0f, CamBody.forward.z).normalized;
            Vector3 LookRight = new Vector3(CamBody.right.x, 0f, CamBody.right.z).normalized;
            Vector3 MoveDir = looKForwad * moveInput.y + LookRight * moveInput.x;

            PlayerBody.forward = looKForwad;
            transform.position += MoveDir * Time.deltaTime * moveSpeed; // 이동 속도 변수로 조정
        }
    }
    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = CamBody.rotation.eulerAngles;

        // 카메라 회전 속도 조정
        float x = camAngle.x - mouseDelta.y * lookSpeed; // 회전 속도 변수로 조정
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f); // 0이 아닌 -1을 최저로 줌으로써 카메라 수평면 아래로 안 내려가는 문제를 방지
        }
        else //180도 보다 큰경우
        {
            x = Mathf.Clamp(x, 335f, 361f); // 로 값을 제한
        }

        CamBody.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x * lookSpeed, camAngle.z); // 회전 속도 변수로 조정
    }
}