using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racket_Attack : MonoBehaviour
{
     public GameObject Effects_electricity;  //이펙트 프리팹 연결
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
        void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mos"))
        {
            //이펙트
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            GameObject effect = Instantiate(Effects_electricity, hitPoint, Quaternion.identity);
            Destroy(effect, 1.5f);
            Debug.Log("임팩트 효과 발동!");
        }
    }
}
