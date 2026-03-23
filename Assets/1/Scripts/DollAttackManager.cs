using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.Rendering;

public class DollAttackManager : MonoBehaviour
{
    [Header("Body Parts")]
    public GameObject headObject;
    public GameObject leftArmObject;
    public GameObject rightArmObject;
    public GameObject leftLegObject;
    public GameObject rightLegObject;

    private GameObject object1;
    private GameObject object2;

    [Header("Body Parts Animator")]
    public Animator headAnimator;
    public Animator leftArmAnimator;
    public Animator rightArmAnimator;
    public Animator leftLegAnimator;
    public Animator rightLegAnimator;

    [Header("AttackUI")]
    public GameObject headAttackUI;
    //public GameObject bodyAttackUI;
    public GameObject leftArmAttackUI;
    public GameObject rightArmAttackUI;
    public GameObject leftLegAttackUI;
    public GameObject rightLegAttackUI;
    public GameObject cprUI;

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

    public SoulMovement soulMovement;
    public bool attacksPausedForCPR = false;

    private Animator animator1;
    private Animator animator2;

    private Animator selectedAnimator1;
    private Animator selectedAnimator2;

    public float hitDuration = 1f;
    public float shakePosIntensity;
    public int shakePosVibration;

    public Coroutine rescuePulseCoroutine;
    public float rescuePulseSpeed = 1.8f;
    public float rescuePulseScaleMin = 0.9f;
    public float rescuePulseScaleMax = 1.15f;
    void Start()
    {
        playerAttackManager = GetComponent<PlayerAttackManager>();

        headAttackUI.SetActive(false);
        //bodyAttackUI.SetActive(false);
        leftArmAttackUI.SetActive(false);
        rightArmAttackUI.SetActive(false);
        leftLegAttackUI.SetActive(false);
        rightLegAttackUI.SetActive(false);

        cprUI.SetActive(false);

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
        if (attacksPausedForCPR || Data.dollHealth <= 0)
            yield break;

        if (attack1 != null) attack1 = null;
        if (attack2 != null) attack2 = null;

        if (selectedAttack1 != null) selectedAttack1 = null;
        if (selectedAttack2 != null) selectedAttack2 = null;

        yield return new WaitForSeconds(attackTimeRange);

        if (attacksPausedForCPR || Data.dollHealth <= 0)
            yield break;

        int isCombo;

        if (Data.isStage2)
        {
            isCombo = Random.Range(0, 2);
        }
        else
        {
            isCombo = 0;
        }

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
        float isSoulFollow = Random.Range(0f, 2f);
        Debug.Log(isSoulFollow);

        if (isSoulFollow <= 1.5f && soulMovement != null)
        {
            soulMovement.SoulMoveToAttack(attack1.attackName);
            Debug.Log("Soul going to attack location");
        }

        isAttacking = true;
        attack1Frozen = false;
        attack2Frozen = false;

        if (attack1 != null)
        {
            SetAnimator(attack1.attackName, ref animator1, ref object1);
        }

        if (attack2 != null)
        {
            SetAnimator(attack2.attackName, ref animator2, ref object2);
        }

        StartCoroutine(playerAttackManager.AttackAnimation());

        while (isAttacking)
        {
            if (attacksPausedForCPR || Data.dollHealth <= 0)
            {
                isAttacking = false;
                yield break;
            }

            bool attackEnded = false;

            if (!attack1Frozen && selectedAttack1 != null && attack1 != null)
            {
                StartCoroutine(playerAttackManager.AttackAnimation());

                ringScale1 -= Vector3.one * Data.ringSpeed * Time.deltaTime;

                attack1.ringObject.transform.localScale = ringScale1;

                if (ringScale1.x <= ringCorrectMax)
                {
                    attack1.isGreen = true;
                    Color c = Color.green;
                    c.a = attack1.ringImage.color.a;
                    attack1.ringImage.color = c;
                }

                if (ringScale1.x <= ringSizeMin)
                {
                    attack1Frozen = true;
                    attackEnded = true;
                    playerAttackManager.WrongTiming(attack1);
                    playerAttackManager.isAttackedFalse = true;
                }
            }

            if (!attack2Frozen && selectedAttack2 != null && attack2 != null)
            {
                ringScale2 -= Vector3.one * Data.ringSpeed * Time.deltaTime;

                attack2.ringObject.transform.localScale = ringScale2;

                if (ringScale2.x <= ringCorrectMax)
                {
                    attack2.isGreen = true;
                    Color c = Color.green;
                    c.a = attack2.ringImage.color.a;
                    attack2.ringImage.color = c;
                }

                if (ringScale2.x <= ringSizeMin)
                {
                    attack2Frozen = true;
                    attackEnded = true;
                    playerAttackManager.WrongTiming(attack2);
                    playerAttackManager.isAttackedFalse = true;
                }
            }

            if (attackEnded)
            {
                isAttacking = false;
            }

            yield return null;
        }
    }

    void SetAnimator(string attackName, ref Animator animator, ref GameObject selectedObject)
    {
        object1 = null;
        object2 = null;

        switch (attackName)
        {
            case "Head":
                if (headAnimator != null) animator = headAnimator;
                selectedObject = headObject;
                TriggerAttackAnimation();
                break;

            case "LeftArm":
                if (leftArmAnimator != null) animator = leftArmAnimator;
                TriggerAttackAnimation();
                selectedObject = leftArmObject;
                break;

            case "RightArm":
                if (rightArmAnimator != null) animator = rightArmAnimator;
                TriggerAttackAnimation();
                selectedObject = rightArmObject;
                break;

            case "LeftLeg":
                if (leftLegAnimator != null) animator = leftLegAnimator;
                TriggerAttackAnimation();
                selectedObject = leftLegObject;
                break;

            case "RightLeg":
                if (rightLegAnimator != null) animator = rightLegAnimator;
                TriggerAttackAnimation();
                selectedObject = leftLegObject;
                break;
        }
    }

    void TriggerAttackAnimation()
    {
        if (animator1 != null) animator1.SetTrigger("Attack");
        if (animator2 != null) animator2.SetTrigger("Attack");
    }

    public void TriggerAnimationEnd()
    {
        if (animator1 != null) animator1.SetTrigger("Hit");
        if (animator2 != null) animator2.SetTrigger("Hit");

        animator1 = null;
        animator2 = null;

        if (object1 != null)
        {
            Vector3 objectPos = object1.transform.position;
            StartCoroutine(DelayBeforeOGPos(object1, objectPos));
            Sequence hit = DOTween.Sequence();
            hit.Append(object1.transform.DOShakePosition(hitDuration, shakePosIntensity, shakePosVibration, 90f, false, false));
            hit.Play();
        }
        if (object2 != null)
        {
            Vector3 objectPos = object2.transform.position;
            StartCoroutine(DelayBeforeOGPos(object2, objectPos));
            Sequence hit = DOTween.Sequence();
            hit.Append(object2.transform.DOShakePosition(hitDuration, shakePosIntensity, shakePosVibration, 90f, false, false));
            hit.Play();
        }
    }

    private IEnumerator DelayBeforeOGPos(GameObject selectedObject, Vector3 objectPos)
    {
        yield return new WaitForSeconds(hitDuration + 0.1f);

        selectedObject.transform.position = objectPos;
    }

    public void EndCurrentAttack()
    {
        if (selectedAttack1 != null && attack1 != null)
        {
            selectedAttack1.SetActive(false);

            ResetAttackColors(attack1);

            attack1.isGreen = false;
            attack1Frozen = false;
        }

        if (selectedAttack2 != null && attack2 != null)
        {
            selectedAttack2.SetActive(false);

            ResetAttackColors(attack2);

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

        if (soulMovement != null)
        {
            soulMovement.SoulRandomMovement();
        }

        if (animator1 != null) animator1 = null;
        if (animator2 != null) animator2 = null;

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

    void ResetAttackColors(ActiveAttack attack)
    {
        Color r = Color.white;
        r.a = attack.ringImage.color.a;
        attack.ringImage.color = r;

        Color c = Color.white;
        c.a = attack.circleImage.color.a;
        attack.circleImage.color = c;
    }

    ActiveAttack CreateAttack(GameObject attackUI)
    {
        GameObject ring = attackUI.transform.GetChild(0).gameObject;
        Image ringImage = ring.GetComponent<Image>();
        Image circleImage = attackUI.transform.GetChild(1).GetComponent<Image>();

        Color r = Color.white;
        r.a = ringImage.color.a;
        ringImage.color = r;

        Color c = Color.white;
        c.a = circleImage.color.a;
        circleImage.color = c;

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

    public void StopAttacksForCPR()
    {
        attacksPausedForCPR = true;
        isAttacking = false;

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

        if (headAttackUI != null) headAttackUI.SetActive(false);
        if (leftArmAttackUI != null) leftArmAttackUI.SetActive(false);
        if (rightArmAttackUI != null) rightArmAttackUI.SetActive(false);
        if (leftLegAttackUI != null) leftLegAttackUI.SetActive(false);
        if (rightLegAttackUI != null) rightLegAttackUI.SetActive(false);

        attack1 = null;
        attack2 = null;
        attack1Frozen = false;
        attack2Frozen = false;
    }

    public void ResumeAttacksAfterCPR()
    {
        if (Data.dollHealth <= 0) return;

        attacksPausedForCPR = false;

        float nextWait = Random.Range(attackTimeRangeMin, attackTimeRangeMax);
        currentAttackCoroutine = StartCoroutine(AttackSequence(nextWait));
    }

    public void StartRescuePulse()
    {
        StopRescuePulse();

        if (headAttackUI != null) headAttackUI.SetActive(true);
        if (leftArmAttackUI != null) leftArmAttackUI.SetActive(true);
        if (rightArmAttackUI != null) rightArmAttackUI.SetActive(true);
        if (leftLegAttackUI != null) leftLegAttackUI.SetActive(true);
        if (rightLegAttackUI != null) rightLegAttackUI.SetActive(true);

        rescuePulseCoroutine = StartCoroutine(RescuePulseRoutine());
    }

    public void StopRescuePulse()
    {
        if (rescuePulseCoroutine != null)
        {
            StopCoroutine(rescuePulseCoroutine);
            rescuePulseCoroutine = null;
        }

        ResetPulseUI(headAttackUI);
        ResetPulseUI(leftArmAttackUI);
        ResetPulseUI(rightArmAttackUI);
        ResetPulseUI(leftLegAttackUI);
        ResetPulseUI(rightLegAttackUI);

        if (headAttackUI != null) headAttackUI.SetActive(false);
        if (leftArmAttackUI != null) leftArmAttackUI.SetActive(false);
        if (rightArmAttackUI != null) rightArmAttackUI.SetActive(false);
        if (leftLegAttackUI != null) leftLegAttackUI.SetActive(false);
        if (rightLegAttackUI != null) rightLegAttackUI.SetActive(false);
    }

    private IEnumerator RescuePulseRoutine()
    {
        while (true)
        {
            float pulse = Mathf.PingPong(Time.time * rescuePulseSpeed, 1f);
            float scale = Mathf.Lerp(rescuePulseScaleMin, rescuePulseScaleMax, pulse);

            PulseSingleUI(headAttackUI, scale);
            PulseSingleUI(leftArmAttackUI, scale);
            PulseSingleUI(rightArmAttackUI, scale);
            PulseSingleUI(leftLegAttackUI, scale);
            PulseSingleUI(rightLegAttackUI, scale);

            yield return null;
        }
    }

    private void PulseSingleUI(GameObject attackUI, float scale)
    {
        if (attackUI == null) return;

        Transform ring = attackUI.transform.GetChild(0);
        Transform circle = attackUI.transform.GetChild(1);

        ring.localScale = new Vector3(scale, scale, scale);
        circle.localScale = new Vector3(scale, scale, scale);

        Image ringImage = ring.GetComponent<Image>();
        Image circleImage = circle.GetComponent<Image>();

        if (ringImage != null)
        {
            Color r = Color.white;
            r.a = Mathf.Lerp(0.45f, 1f, Mathf.PingPong(Time.time * rescuePulseSpeed, 1f));
            ringImage.color = r;
        }

        if (circleImage != null)
        {
            Color c = Color.white;
            c.a = Mathf.Lerp(0.25f, 0.8f, Mathf.PingPong(Time.time * rescuePulseSpeed, 1f));
            circleImage.color = c;
        }
    }

    private void ResetPulseUI(GameObject attackUI)
    {
        if (attackUI == null) return;

        Transform ring = attackUI.transform.GetChild(0);
        Transform circle = attackUI.transform.GetChild(1);

        ring.localScale = Vector3.one;
        circle.localScale = Vector3.one;

        Image ringImage = ring.GetComponent<Image>();
        Image circleImage = circle.GetComponent<Image>();

        if (ringImage != null)
        {
            Color r = Color.white;
            r.a = 1f;
            ringImage.color = r;
        }

        if (circleImage != null)
        {
            Color c = Color.white;
            c.a = 1f;
            circleImage.color = c;
        }
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
    public bool wasHit = false;
}



