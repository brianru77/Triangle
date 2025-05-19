using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Animator anime;
    private int comboStep = 0;
    private float lastClickTime = 0f;
    private float comboDelay = 0.8f; // 콤보 입력 유효 시간
    private bool canReceiveInput = true;

    void Start()
    {
        anime = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleComboInput();
        }

        // 시간이 지나면 콤보 초기화
        if (Time.time - lastClickTime > comboDelay)
        {
            ResetCombo();
        }
    }
    void HandleComboInput()
    {
        if (!canReceiveInput) return;

        lastClickTime = Time.time;
        comboStep++;

        if (comboStep == 1)
        {
            Debug.Log("1타");
            anime.Play("Martelo 2");
        }
        // else if (comboStep == 2)
        // {
        //     Debug.Log("2타");
        //     anime.Play("Boxing");
        // }
        // else if (comboStep == 3)
        // {
        //     Debug.Log("3타");
        //     anime.Play("Martelo 2");
        //     comboStep = 0; // 마지막 콤보까지 끝나면 초기화
        // }

        canReceiveInput = false;
    }

    // 애니메이션 이벤트에서 호출
    public void EnableComboInput()
    {
        canReceiveInput = true;
    }

    void ResetCombo()
    {
        comboStep = 0;
        canReceiveInput = true;
    }
}
