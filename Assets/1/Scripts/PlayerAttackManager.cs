using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackManager : MonoBehaviour
{
    public int attackAmount = 5;
    public bool isBeingAttacked = false;

    public float penaltyWaitTime = 1.5f;
    public float correctWaitTime = 0.5f;

    private DollHealthManager dollHealthManager;
    private DollAttackManager dollAttackManager;

    void Start()
    {
        dollHealthManager = GetComponent<DollHealthManager>();
        dollAttackManager = GetComponent<DollAttackManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) HandleAttack("Head");
        else if (Input.GetKeyDown(KeyCode.S)) HandleAttack("Body");
        else if (Input.GetKeyDown(KeyCode.A)) HandleAttack("LeftArm");
        else if (Input.GetKeyDown(KeyCode.D)) HandleAttack("RightArm");
        else if (Input.GetKeyDown(KeyCode.Z)) HandleAttack("LeftLeg");
        else if (Input.GetKeyDown(KeyCode.X)) HandleAttack("RightLeg");
    }

    void HandleAttack(string attack)
    {
        bool hasHit = false;
        bool hitSuccessful = false;

        if (dollAttackManager.attack1 != null && dollAttackManager.attack1.attackName == attack)
        {
            if (dollAttackManager.attack1.isGreen)
            {
                hitSuccessful = true;
                DollGotHit();
                dollAttackManager.attack1Frozen = true;
                StartCoroutine(DissapearAfterDelay(correctWaitTime));
            }
            else
            {
                WrongTiming(dollAttackManager.attack1);
            }
        }
        else
        {
            hasHit = true;
            DollGotHit(); //If there is no attack happening, doll loses health
        }

        if (dollAttackManager.attack2 != null && dollAttackManager.attack2.attackName == attack)
        {
            if (dollAttackManager.attack2.isGreen)
            {
                hitSuccessful = true;
                DollGotHit();
                dollAttackManager.attack2Frozen = true;
                StartCoroutine(DissapearAfterDelay(correctWaitTime));
            }
            else
            {
                WrongTiming(dollAttackManager.attack2);
            }
        }

        if (!hitSuccessful && !hasHit) //If the player pressed the wrong attack button, doll still loses health
        {
            DollGotHit();
        }

    }

    public void WrongTiming(ActiveAttack attack)
    {
        attack.isGreen = false;
        dollAttackManager.attack1.circleImage.color = Color.red;
        dollAttackManager.attack1.ringImage.color = Color.red;
        dollAttackManager.attack1Frozen = true;

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

    void DollGotHit()
    {
        dollHealthManager.loseHealth(attackAmount);
    }
}
