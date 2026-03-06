using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public bool isAttacking = false;

    private GameObject selectedAttack1;
    private GameObject selectedAttack2;

    private GameObject selectedAttack1Ring;
    private GameObject selectedAttack2Ring;

    private Image selectedAttack1RingImage;
    private Image selectedAttack2RingImage;

    private Image selectedAttack1CircleImage;
    private Image selectedAttack2CircleImage;

    public ActiveAttack attack1;
    public ActiveAttack attack2;

    public bool attack1Frozen;
    public bool attack2Frozen;

    public Coroutine currentAttackCoroutine;

    private PlayerAttackManager playerAttackManager;

    void Start()
    {
        playerAttackManager = GetComponent<PlayerAttackManager>();

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

        currentAttackCoroutine = StartCoroutine(AttackSequence(attackStartTime));
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
        attack1Frozen = false;
        attack2Frozen = false;

        Vector3 ringScale1 = new Vector3(ringSizeMax, ringSizeMax, ringSizeMax);
        Vector3 ringScale2 = new Vector3(ringSizeMax, ringSizeMax, ringSizeMax);

        if (selectedAttack1 != null)
        {
            selectedAttack1.SetActive(true);

            selectedAttack1Ring = selectedAttack1.transform.GetChild(0).gameObject;
            selectedAttack1RingImage = selectedAttack1Ring.GetComponent<Image>();
            selectedAttack1CircleImage = selectedAttack1.transform.GetChild(1).gameObject.GetComponent<Image>();
            selectedAttack1RingImage.color = Color.white;
            selectedAttack1CircleImage.color = Color.white;

            attack1 = new ActiveAttack
            {
                attackObject = selectedAttack1,
                ringObject = selectedAttack1Ring,
                ringImage = selectedAttack1RingImage,
                circleImage = selectedAttack1CircleImage,
                attackName = GetKeyForAttack(selectedAttack1.name),
                isGreen = false
            };
        }

        if (selectedAttack2 != null)
        {
            selectedAttack2.SetActive(true);
            selectedAttack2Ring = selectedAttack2.transform.GetChild(0).gameObject;
            selectedAttack2RingImage = selectedAttack2Ring.GetComponent<Image>();
            selectedAttack2CircleImage = selectedAttack2.transform.GetChild(1).gameObject.GetComponent<Image>();
            selectedAttack2RingImage.color = Color.white;
            selectedAttack2CircleImage.color = Color.white;

            attack2 = new ActiveAttack
            {
                attackObject = selectedAttack2,
                ringObject = selectedAttack2Ring,
                ringImage = selectedAttack2RingImage,
                circleImage = selectedAttack2CircleImage,
                attackName = GetKeyForAttack(selectedAttack2.name),
                isGreen = false
            };
        }

        while (isAttacking)
        {
            bool attackEnded = false;

            if (!attack1Frozen && selectedAttack1 != null)
            {
                ringScale1 -= Vector3.one * Data.ringSpeed * Time.deltaTime;

                selectedAttack1Ring.transform.localScale = ringScale1;

                if (ringScale1.x <= 1.3f)
                {
                    attack1.isGreen = true;
                    selectedAttack1RingImage.color = Color.green;
                }

                if (ringScale1.x <= ringSizeMin)
                {
                    attack1Frozen = true;
                    attackEnded = true;
                    playerAttackManager.WrongTiming(attack1);
                }
            }

            if (selectedAttack2 != null && !attack2Frozen)
            {
                ringScale2 -= Vector3.one * Data.ringSpeed * Time.deltaTime;

                selectedAttack2Ring.transform.localScale = ringScale2;

                if (ringScale2.x <= 1.3f)
                {
                    attack2.isGreen = true;
                    selectedAttack2RingImage.color = Color.green;
                }

                if (ringScale2.x <= ringSizeMin)
                {
                    attack1Frozen = true;
                    attackEnded = true;
                    playerAttackManager.WrongTiming(attack2);
                }
            }

            if (attackEnded)
            {
                isAttacking = false;
            }

            yield return null; 
        }
    }

    public void EndCurrentAttack()
    {
        if (selectedAttack1 != null)
        {
            selectedAttack1.SetActive(false);
            selectedAttack1RingImage.color = Color.white;
            selectedAttack1CircleImage.color = Color.white;
            attack1.isGreen = false;
            attack1Frozen = false;
        }

        if (selectedAttack2 != null)
        {
            selectedAttack2.SetActive(false);
            selectedAttack2RingImage.color = Color.white;
            selectedAttack2CircleImage.color = Color.white;
            attack2.isGreen = false;
            attack2Frozen = false;
        }

        if (currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
            currentAttackCoroutine = null;
        }

        float nextWait = Random.Range(attackTimeRangeMin, attackTimeRangeMax);
        currentAttackCoroutine = StartCoroutine(AttackSequence(nextWait));
    }

    string GetKeyForAttack(string attackName)
    {
        switch (attackName)
        {
            case "HeadAttackUI": return "Head";
            case "BodyAttackUI": return "Body";
            case "LeftArmAttackUI": return "LeftArm";
            case "RightArmAttackUI": return "RightArm";
            case "LeftLegAttackUI": return "LeftLeg";
            case "RightLegAttackUI": return "RightLeg";
        }
        return "";
    }
}

public class ActiveAttack
{
    public GameObject attackObject;
    public GameObject ringObject;
    public Image ringImage;
    public Image circleImage;
    public string attackName; 
    public bool isGreen;
}

