using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    public RectTransform aimTransform;  // 에임 UI 이미지의 RectTransform

    void Update()
    {
        // 마우스의 화면 좌표를 가져와서 에임 위치를 설정
        Vector2 mousePosition = Input.mousePosition;
        aimTransform.position = mousePosition;
    }
}
