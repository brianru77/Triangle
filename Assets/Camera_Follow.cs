using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    public Transform Player; //따라 가야할 오브젝트의 정보
    public float Cam_Follow_Speed = 10;
    public float Mouse_sensitivity = 100; //마우스 감도
    public float Angle_limit = 70; //카메라 각도 제한
    public float smoothness = 10f;

    private float getX; //입력 받을 x축 값
    private float getY; //입력 받을 y축 값

    public Transform UnityCam; //실제 카메라
    public Vector3 dirNomalize;
    public Vector3 Final_V3_Values;
    public float Min_Distance; //장애물을 피한 뒤 플레이어와의 붙는 가장 가까운거리
    public float Max_Distance; //최대한 먼거리
    public float Final_Distance;
    // Start is called before the first frame update
    void Start()
    {
        getX = transform.localRotation.eulerAngles.x; //초기 값은 초기화
        getY = transform.localRotation.eulerAngles.y;
        dirNomalize = UnityCam.localPosition.normalized;
        Final_Distance = UnityCam.localPosition.magnitude;

    }

    // Update is called once per frame
    void Update()
    {
        getX += -(Input.GetAxis("Mouse Y")) * Mouse_sensitivity * Time.deltaTime;
        getY += Input.GetAxis("Mouse X") * Mouse_sensitivity * Time.deltaTime;

        getX = Math.Clamp(getX, -Angle_limit, Angle_limit);
        Quaternion rot = Quaternion.Euler(getX, getY, 0);
        transform.rotation = rot;

    }
    void LateUpdate() //카메라의 움직임
    {
        transform.position = Vector3.MoveTowards(transform.position, Player.position, Cam_Follow_Speed * Time.deltaTime);
        Final_V3_Values = transform.TransformPoint(dirNomalize * Max_Distance);
        //장애물 감지
        RaycastHit hit;
        if (Physics.Linecast(transform.position, Final_V3_Values, out hit))
        {
            Final_Distance = Math.Clamp(hit.distance, Min_Distance, Max_Distance);
        }
        else
        {
            Final_Distance = Max_Distance;
        }
        UnityCam.localPosition = Vector3.Lerp(UnityCam.localPosition, dirNomalize * Final_Distance, Time.deltaTime * smoothness);
    }
}
