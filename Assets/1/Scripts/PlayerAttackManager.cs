using System.Collections;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    public int dollAttackAmount = 5;
    public int playerAttackAmount = 10;
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

    void Start()
    {
        playerHealthManager = GetComponent<PlayerHealthManager>();
        dollHealthManager = GetComponent<DollHealthManager>();
        dollAttackManager = GetComponent<DollAttackManager>();
        cprManager = GetComponent<CPR>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) HandleAttack("Head");
        else if (Input.GetKeyDown(KeyCode.A)) HandleAttack("LeftArm");
        else if (Input.GetKeyDown(KeyCode.D)) HandleAttack("RightArm");
        else if (Input.GetKeyDown(KeyCode.Z)) HandleAttack("LeftLeg");
        else if (Input.GetKeyDown(KeyCode.X)) HandleAttack("RightLeg");

        if (Input.GetKeyDown(KeyCode.S))
        {
            HandleCPR();
        }
    }

    void HandleAttack(string attack)
    {
        bool hasHit = false;
        bool hitSuccessful = false;

        isAttackedFalse = false;

        if (dollAttackManager.attack1 != null && dollAttackManager.attack1.attackName == attack)
        {
            if (dollAttackManager.attack1.isGreen)
            {
                isAttackedFalse = false;
                hitSuccessful = true;
                DollGotHit();
                dollAttackManager.attack1Frozen = true;
                StartCoroutine(DissapearAfterDelay(correctWaitTime));

                Debug.Log(attack + " : successfully hit!");
            }
            else
            {
                isAttackedFalse = true;
                hasHit = true;
                WrongTiming(dollAttackManager.attack1);
                Debug.Log(attack + " : wrong timing!");
            }
        }

        if (dollAttackManager.attack2 != null && dollAttackManager.attack2.attackName == attack)
        {
            if (dollAttackManager.attack2.isGreen)
            {
                isAttackedFalse = false;
                hitSuccessful = true;
                DollGotHit();
                dollAttackManager.attack2Frozen = true;
                StartCoroutine(DissapearAfterDelay(correctWaitTime));
                Debug.Log(attack + " : successfully hit!");
            }
            else
            {
                isAttackedFalse = true;
                hasHit = true;
                WrongTiming(dollAttackManager.attack2);
                Debug.Log(attack + " : wrong timing!");
            }
        }

        //if (!hitSuccessful && !hasHit)
        //{
        //    DollGotHit();
        //    Debug.Log(attack + " : hit!");
        //}
    }

    void HandleCPR()
    {
        if (cprManager == null) return;
        if (!cprManager.cprActive) return;
        if (cprManager.cprAttack == null) return;

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
        cprManager.cprAttack.circleImage.color = Color.red;
        cprManager.cprAttack.ringImage.color = Color.red;
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
            dollAttackManager.attack1.circleImage.color = Color.red;
            dollAttackManager.attack1.ringImage.color = Color.red;
            dollAttackManager.attack1Frozen = true;
        }

        if (dollAttackManager.attack2 != null)
        {
            dollAttackManager.attack2.circleImage.color = Color.red;
            dollAttackManager.attack2.ringImage.color = Color.red;
            dollAttackManager.attack2Frozen = true;
        }

        StartCoroutine(DissapearAfterDelay(penaltyWaitTime));
    }

    private IEnumerator DissapearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        dollAttackManager.EndCurrentAttack();
    }

    public IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(attackAnimationDuration);

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