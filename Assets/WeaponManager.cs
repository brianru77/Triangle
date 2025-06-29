using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("무기 소켓 위치 (손뼈의 자식 오브젝트)")]
    public Transform weaponSocket;

    [Header("장착할 무기 프리팹")]
    public GameObject weaponPrefab;

    private GameObject currentWeapon;

    void Start()
    {
        EquipWeapon();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentWeapon == null)
            {
                EquipWeapon();   //무기가 없으면 장착
            }
            else
            {
                UnequipWeapon(); //무기가 있으면 해제
            }
        }
    }

    //무기 장착 함수
    public void EquipWeapon()
    {
        //기존 무기가 있다면 제거
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        //무기 생성 후 소켓에 붙이기
        currentWeapon = Instantiate(weaponPrefab, weaponSocket);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
    }
    //무기 해제
    public void UnequipWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }
    }
}
