using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchHit : MonoBehaviour
{
    public GameObject PunchEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mos"))
        {
            //이펙트
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            GameObject effect = Instantiate(PunchEffect, hitPoint, Quaternion.identity);
            Destroy(effect, 1.5f);
            Debug.Log("임팩트 효과 발동!");
        }
    }
}
