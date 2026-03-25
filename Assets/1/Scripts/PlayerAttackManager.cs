using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    ArduinoInput ai;

    int threshold = 50;
    bool headPressedLast;
    bool leftArmPressedLast;
    bool rightArmPressedLast;
    bool leftLegPressedLast;
    bool rightLegPressedLast;
    bool bodyPressedLast;
    private int dollAttackAmount = 5;
    public bool isBeingAttacked = false;
    public bool isAttackedFalse = false;

    public float penaltyWaitTime = 1.5f;
    public float correctWaitTime = 0.5f;

    public float attackAnimationDuration = 2f;

    private PlayerHealthManager playerHealthManager;
    private DollHealthManager dollHealthManager;
    private DollAttackManager dollAttackManager;
    private CPR cprManager;

    private float dollDamageWindow = 0.2f;
    private float lastDollDamageTime = -1f;

    private bool comboActive = false;       
    private bool comboFailed = false;     
    private float comboStartTime = 0f;      
    public float comboWindow = 0.2f;    
    private List<ActiveAttack> currentCombo = new List<ActiveAttack>();

    public SoulLivesManager soulLivesManager;

    private int playerAttackAmount = 15;
    private int dollAttackAmountSoft = 45;
    private int dollAttackAmountMedium = 9;
    private int dollAttackAmountStrong = 12;

    private CPR cpr;
    private SoulExtract soulExtract;

    void Start()
    {
        ai = GetComponent<ArduinoInput>();
        cpr = GetComponent<CPR>();
        playerHealthManager = GetComponent<PlayerHealthManager>();
        dollHealthManager = GetComponent<DollHealthManager>();
        dollAttackManager = GetComponent<DollAttackManager>();
        cprManager = GetComponent<CPR>();
        soulExtract = GetComponent<SoulExtract>();


    }

    void Update()
{
    bool head = ai.head > threshold;
    bool leftArm = ai.leftArm > threshold;
    bool rightArm = ai.rightArm > threshold;
    bool leftLeg = ai.leftLeg > threshold;
    bool rightLeg = ai.rightLeg > threshold;
    bool body = ai.body > threshold;

    if (head && !headPressedLast) CheckAttackStrength("Head");
    if (leftArm && !leftArmPressedLast) CheckAttackStrength("LeftArm");
    if (rightArm && !rightArmPressedLast) CheckAttackStrength("RightArm");
    if (leftLeg && !leftLegPressedLast) CheckAttackStrength("LeftLeg");
    if (rightLeg && !rightLegPressedLast) CheckAttackStrength("RightLeg");

    if (body && !bodyPressedLast) HandleCPR();

    headPressedLast = head;
    leftArmPressedLast = leftArm;
    rightArmPressedLast = rightArm;
    leftLegPressedLast = leftLeg;
    rightLegPressedLast = rightLeg;
    bodyPressedLast = body;
}

void CheckAttackStrength(string attackKey)
{
    int pressure = 0;

    switch (attackKey)
    {
        case "Head": pressure = ai.head; break;
        case "LeftArm": pressure = ai.leftArm; break;
        case "RightArm": pressure = ai.rightArm; break;
        case "LeftLeg": pressure = ai.leftLeg; break;
        case "RightLeg": pressure = ai.rightLeg; break;
    }

    Data.isHitSoft = false;
    Data.isHitMedium = false;
    Data.isHitStrong = false;

    if (pressure < 300)
    {
        Data.isHitSoft = true;
        playerAttackAmount = dollAttackAmountSoft;
    }
    else if (pressure < 700)
    {
        Data.isHitMedium = true;
        playerAttackAmount = dollAttackAmountMedium;
    }
    else
    {
        Data.isHitStrong = true;
        playerAttackAmount = dollAttackAmountStrong;

        cpr.StartCPR();
    }

    HandleAttack(attackKey);
}

    void HandleAttack(string attackKey)
    {
        bool matchedAttack = false;

        if (dollAttackManager.attack1 != null && dollAttackManager.attack1.attackName == attackKey)
            matchedAttack = true;

        if (dollAttackManager.attack2 != null && dollAttackManager.attack2.attackName == attackKey)
            matchedAttack = true;

        if (soulLivesManager != null) soulLivesManager.HitSoul(attackKey);

        if (!matchedAttack && (dollAttackManager.attack1 != null || dollAttackManager.attack2 != null))
        {
            Debug.Log("Wrong button pressed!");

            comboFailed = true;
            isAttackedFalse = true;

            if (dollAttackManager.attack1 != null)
            {
                if (dollAttackManager.attack2 != null) dollAttackManager.attack2.isGreen = false;
                WrongTiming(dollAttackManager.attack1);
            }

            return;
        }

        //bool hasHit = false;
        //bool hitSuccessful = false;

        isAttackedFalse = false;

        if (dollAttackManager.attack1 != null && dollAttackManager.attack2 != null)
        {
            if (!comboActive)
            {
                comboActive = true;
                comboFailed = false;
                comboStartTime = Time.time;

                currentCombo.Clear();
                currentCombo.Add(dollAttackManager.attack1);
                currentCombo.Add(dollAttackManager.attack2);
            }

            foreach (var comboAttack in currentCombo)
            {
                if (comboAttack.attackName == attackKey)
                {
                    if (comboAttack.isGreen)
                    {
                        dollAttackManager.TriggerAnimationEnd();
                        comboAttack.wasHit = true;
                        Debug.Log(attackKey + " : hit correctly");

                        if (comboAttack == dollAttackManager.attack1)
                        {
                            ComboFreeze(1);
                        }
                        else
                        {
                            ComboFreeze(0);
                        }
                    }
                    else
                    {
                        comboFailed = true;
                        Debug.Log(attackKey + " : wrong timing → combo fail");
                    }
                }
            }

            bool allHit = true;
            foreach (var comboAttack in currentCombo)
            {
                if (!comboAttack.wasHit) allHit = false;
            }

            // Resolve combo if window expired or all attacks hit
            if (Time.time - comboStartTime > comboWindow || allHit)
            {
                ResolveCombo();
            }
        }
        else // Single attack
        {
            HandleSingleAttack(attackKey);
        }
    }

    void HandleSingleAttack(string attackKey)
    {
        if (dollAttackManager.attack1 != null && dollAttackManager.attack1.attackName == attackKey)
        {
            if (dollAttackManager.attack1.isGreen)
            {
                dollAttackManager.TriggerAnimationEnd();
                DollGotHit();
                dollAttackManager.attack1Frozen = true;
                StartCoroutine(DissapearAfterDelay(correctWaitTime));
                Debug.Log(attackKey + " : successfully hit!");
            }
            else
            {
                isAttackedFalse = true;
                WrongTiming(dollAttackManager.attack1);
                Debug.Log(attackKey + " : wrong timing");
            }
        }

        if (dollAttackManager.attack2 != null && dollAttackManager.attack2.attackName == attackKey)
        {
            if (dollAttackManager.attack2.isGreen)
            {
                dollAttackManager.TriggerAnimationEnd();
                DollGotHit();
                dollAttackManager.attack2Frozen = true;
                StartCoroutine(DissapearAfterDelay(correctWaitTime));
                Debug.Log(attackKey + " : successfully hit!");
            }
            else
            {
                isAttackedFalse = true;
                WrongTiming(dollAttackManager.attack2);
                Debug.Log(attackKey + " : wrong timing!");
            }
        }
    }

    void ResolveCombo()
    {
        if (comboFailed)
        {
            dollAttackManager.attack2.isGreen = false;
            WrongTiming(dollAttackManager.attack1);
            Debug.Log("Combo failed → player loses health");
            playerHealthManager.loseHealth(playerAttackAmount);
        }
        else
        {
            Debug.Log("Combo successful → doll loses health once");
            StartCoroutine(DissapearAfterDelay(correctWaitTime));
            DollGotHit();
        }

        // Reset combo
        foreach (var attack in currentCombo)
        {
            attack.wasHit = false;
        }
        comboActive = false;
        comboFailed = false;
        currentCombo.Clear();
    }

    void HandleCPR()
    {
        if (cprManager == null) return;
        if (!cprManager.cprActive) return;
        if (cprManager.cprAttack == null) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            cpr.CPRFail();
            return;
        }

        if (cprManager.cprAttack.isGreen)
        {
            cprManager.CPRSuccess();
            Debug.Log("CPR success!");
        }
        else
        {
            CPRWrongTiming();
            Debug.Log("CPR wrong timing!");
        }
    }

    void CPRWrongTiming()
    {
        if (cprManager == null || cprManager.cprAttack == null) return;

        cprManager.cprAttack.isGreen = false;

        Color c = Color.red;
        c.a = cprManager.cprAttack.circleImage.color.a;
        cprManager.cprAttack.circleImage.color = c;

        Color r = Color.red;
        r.a = cprManager.cprAttack.ringImage.color.a;
        cprManager.cprAttack.ringImage.color = r;

        cprManager.cprFrozen = true;

        StartCoroutine(CPRDisappearAfterDelay(penaltyWaitTime));
    }

    IEnumerator CPRDisappearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (cprManager != null)
        {
            cprManager.EndCurrentCPR();
        }
    }

    public void WrongTiming(ActiveAttack attack)
    {
        attack.isGreen = false;

        if (dollAttackManager.attack1 != null)
        {
            Color c = Color.red;
            c.a = dollAttackManager.attack1.circleImage.color.a;
            dollAttackManager.attack1.circleImage.color = c;

            Color r = Color.red;
            r.a = dollAttackManager.attack1.ringImage.color.a;
            dollAttackManager.attack1.ringImage.color = r;

            dollAttackManager.attack1Frozen = true;
        }

        if (dollAttackManager.attack2 != null)
        {
            Color c = Color.red;
            c.a = dollAttackManager.attack2.circleImage.color.a;
            dollAttackManager.attack2.circleImage.color = c;

            Color r = Color.red;
            r.a = dollAttackManager.attack2.ringImage.color.a;
            dollAttackManager.attack2.ringImage.color = r;

            dollAttackManager.attack2Frozen = true;
        }

        StartCoroutine(DissapearAfterDelay(penaltyWaitTime));
    }

    private IEnumerator DissapearAfterDelay(float delay)
    {
        if (dollAttackManager.attack1 != null) dollAttackManager.attack1Frozen = true;

        if (dollAttackManager.attack2 != null) dollAttackManager.attack2Frozen = true;

        yield return new WaitForSeconds(delay);

        dollAttackManager.EndCurrentAttack();
    }

    void ComboFreeze(int isFirst)
    {
        if (isFirst == 0)
        {
            if (dollAttackManager.attack2 != null) dollAttackManager.attack2Frozen = true;
        }
        else
        {
            if (dollAttackManager.attack1 != null) dollAttackManager.attack1Frozen = true;
        }
    }

    public IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(attackAnimationDuration);

        if (soulExtract != null && soulExtract.rescueActive)
            yield break;

        if (dollAttackManager != null && dollAttackManager.attacksPausedForCPR)
            yield break;

        if (isAttackedFalse)
        {
            Debug.Log("player losing health");

            if (dollAttackManager.attack2 != null)
            {
                playerHealthManager.loseHealth(playerAttackAmount * 2);
            }
            else
            {
                playerHealthManager.loseHealth(playerAttackAmount);
            }

            isAttackedFalse = false;
        }
    }

    void DollGotHit()
    {
        if (Time.time - lastDollDamageTime < dollDamageWindow)
        {
            return;
        }

        lastDollDamageTime = Time.time;
        dollHealthManager.loseHealth(dollAttackAmount);
    }

}