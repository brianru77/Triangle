using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformation : MonoBehaviour
{
    public int transform_level = 0; //0평소 1중간혈압 2고혈압
    private Animator anime;

    public GameObject Transform_Effect1;
    public GameObject Transform_Effect12;
    public GameObject Transform_final_level_Effect;

    void Start()
    {
        anime = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (transform_level < 2)
            {
                transform_level++;
                StartCoroutine(TransformSequence(transform_level));
            }
        }
    }

    IEnumerator TransformSequence(int level)
    {
        //애니메이터에 단계 전달
        anime.SetInteger("Transform_level", level);

        //이펙트 출력
        if (level == 1 && Transform_Effect1 != null)
        {
            GameObject effect = Instantiate(Transform_Effect1, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform); //따라다니게 설정
            effect.transform.localPosition = new Vector3(0, 0, 0); //위치 발 밑으로
            effect.transform.localRotation = Quaternion.identity;
        }
        else if (level == 2 && Transform_Effect12 != null)
        {
            GameObject effect1 = Instantiate(Transform_Effect12, transform.position, Quaternion.identity);
            effect1.transform.SetParent(transform);
            effect1.transform.localPosition = new Vector3(0, 0.7f, 0);
            effect1.transform.localRotation = Quaternion.identity;

            GameObject effect2 = Instantiate(Transform_final_level_Effect, transform.position, Quaternion.identity);
            effect2.transform.SetParent(transform);
            effect2.transform.localPosition = new Vector3(0, 0, 0);
            effect2.transform.localRotation = Quaternion.identity;

            Destroy(effect2, 2f); //폭발 임팩트는 일시적
        }

        yield return new WaitForSeconds(1.5f);
    }
}

