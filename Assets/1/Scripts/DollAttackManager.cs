using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DollAttackManager : MonoBehaviour
{
    [Header("AttackUI")]
    public GameObject headAttackUI;
    //public GameObject bodyAttackUI;
    public GameObject leftArmAttackUI;
    public GameObject rightArmAttackUI;
    public GameObject leftLegAttackUI;
    public GameObject rightLegAttackUI;

    private List<GameObject> attackUIList;

    public float ringSizeMin = 0.9f;
    public float ringSizeMax = 2f;

    [Header("Attack Times")]
    public float attackStartTime = 3f;
    public float attackTimeRangeMin = 3f;
    public float attackTimeRangeMax = 7f;

    private bool isAttacking = false;

    private GameObject selectedAttack1;
    private GameObject selectedAttack2;

    private GameObject selectedAttack1Ring;
    private GameObject selectedAttack2Ring;

    void Start()
    {
        headAttackUI.SetActive(false);
        //bodyAttackUI.SetActive(false);
        leftArmAttackUI.SetActive(false);
        rightArmAttackUI.SetActive(false);
        leftLegAttackUI.SetActive(false);
        rightLegAttackUI.SetActive(false);

        attackUIList = new List<GameObject>()
        {
            headAttackUI,
            //bodyAttackUI,
            leftArmAttackUI,
            rightArmAttackUI,
            leftLegAttackUI,
            rightLegAttackUI
        };

        StartCoroutine(AttackSequence(attackStartTime));
    }

    private IEnumerator AttackSequence(float attackTimeRange)
    {
        yield return new WaitForSeconds(attackTimeRange);

        //Randomly select one or two attacks
        int isCombo = Random.Range(0, 2);

        if (isCombo == 0)
        {
            int randomIndex = Random.Range(0, attackUIList.Count);
            selectedAttack1 = attackUIList[randomIndex];
            selectedAttack2 = null;
        }
        else
        {
            int firstIndex = Random.Range(0, attackUIList.Count);
            int secondIndex;

            do
            {
                secondIndex = Random.Range(0, attackUIList.Count);
            }
            while (secondIndex == firstIndex);

            selectedAttack1 = attackUIList[firstIndex];
            selectedAttack2 = attackUIList[secondIndex];
        }

        StartCoroutine(StartAttack());
    }

    private IEnumerator StartAttack()
    {
        isAttacking = true;
        Vector3 ringScale = new Vector3(ringSizeMax, ringSizeMax, ringSizeMax);

        if (selectedAttack2 == null)
        {
            selectedAttack1.SetActive(true);

            selectedAttack1Ring = selectedAttack1.transform.GetChild(0).gameObject;
        }
        else
        {
            selectedAttack1.SetActive(true);
            selectedAttack2.SetActive(true);

            selectedAttack1Ring = selectedAttack1.transform.GetChild(0).gameObject;
            selectedAttack2Ring = selectedAttack2.transform.GetChild(0).gameObject;
        }

        while (isAttacking)
        {
            ringScale -= Vector3.one * Data.ringSpeed * Time.deltaTime;
            if (ringScale.x <= ringSizeMin)
            {
                ringScale = Vector3.one * ringSizeMin;

                selectedAttack1.SetActive(false);
                if (selectedAttack2 != null) selectedAttack2.SetActive(false);

                float randomWaitTime = Random.Range(attackTimeRangeMin, attackTimeRangeMax);
                StartCoroutine(AttackSequence(randomWaitTime));

                isAttacking = false;
            }

            selectedAttack1Ring.transform.localScale = ringScale;
            if (selectedAttack2 != null) selectedAttack2Ring.transform.localScale = ringScale;

            yield return null; 
        }
    }

    void Update()
    {
        
    }
}
