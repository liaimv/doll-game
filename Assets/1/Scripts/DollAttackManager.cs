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

    public float ringCorrectMax = 1.3f;

    private Vector3 ringScale1;
    private Vector3 ringScale2;

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
    public Coroutine currentStartAttackCoroutine;

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
        if (attack1 != null) attack1 = null;
        if (attack2 != null) attack2 = null;

        if (selectedAttack1 != null) selectedAttack1 = null;
        if (selectedAttack2 != null) selectedAttack2 = null;

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

        CreateAttack();
        currentStartAttackCoroutine = StartCoroutine(StartAttack());
    }

    private void CreateAttack()
    {
        if (selectedAttack1 != null)
        {
            selectedAttack1.SetActive(true);
            attack1 = CreateAttack(selectedAttack1);
        }

        if (selectedAttack2 != null)
        {
            selectedAttack2.SetActive(true);
            attack2 = CreateAttack(selectedAttack2);
        }

        ringScale1 = new Vector3(ringSizeMax, ringSizeMax, ringSizeMax);
        ringScale2 = new Vector3(ringSizeMax, ringSizeMax, ringSizeMax);
    }

    private IEnumerator StartAttack()
    {
        isAttacking = true;
        attack1Frozen = false;
        attack2Frozen = false;

        while (isAttacking)
        {
            bool attackEnded = false;

            if (!attack1Frozen && selectedAttack1 != null && attack1 != null)
            {
                ringScale1 -= Vector3.one * Data.ringSpeed * Time.deltaTime;

                attack1.ringObject.transform.localScale = ringScale1;

                if (ringScale1.x <= ringCorrectMax)
                {
                    attack1.isGreen = true;
                    attack1.ringImage.color = Color.green;
                }

                if (ringScale1.x <= ringSizeMin)
                {
                    attack1Frozen = true;
                    attackEnded = true;
                    playerAttackManager.WrongTiming(attack1);
                }
            }

            if (!attack2Frozen && selectedAttack2 != null && attack2 != null)
            {
                ringScale2 -= Vector3.one * Data.ringSpeed * Time.deltaTime;

                attack2.ringObject.transform.localScale = ringScale2;

                if (ringScale2.x <= ringCorrectMax)
                {
                    attack2.isGreen = true;
                    attack2.ringImage.color = Color.green;
                }

                if (ringScale2.x <= ringSizeMin)
                {
                    attack2Frozen = true;
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
        if (selectedAttack1 != null && attack1 != null)
        {
            selectedAttack1.SetActive(false);
            attack1.ringImage.color = Color.white;
            attack1.circleImage.color = Color.white;
            attack1.isGreen = false;
            attack1Frozen = false;
        }

        if (selectedAttack2 != null && attack2 != null)
        {
            selectedAttack2.SetActive(false);
            attack2.ringImage.color = Color.white;
            attack2.circleImage.color = Color.white;
            attack2.isGreen = false;
            attack2Frozen = false;
        }

        if (currentStartAttackCoroutine != null)
        {
            StopCoroutine(currentStartAttackCoroutine);
            currentStartAttackCoroutine = null;
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

    ActiveAttack CreateAttack(GameObject attackUI)
    {
        GameObject ring = attackUI.transform.GetChild(0).gameObject;
        Image ringImage = ring.GetComponent<Image>();
        Image circleImage = attackUI.transform.GetChild(1).GetComponent<Image>();

        ringImage.color = Color.white;
        circleImage.color = Color.white;

        return new ActiveAttack
        {
            attackObject = attackUI,
            ringObject = ring,
            ringImage = ringImage,
            circleImage = circleImage,
            attackName = GetKeyForAttack(attackUI.name),
            isGreen = false
        };
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



