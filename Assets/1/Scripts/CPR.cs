using UnityEngine;
using UnityEngine.UI;

public class CPR : MonoBehaviour
{
    public float cprThreshold = 10f;
    public int pressesNeeded = 3;
    public float healAmount = 20f;

    private int successfulPresses = 0;

    private DollAttackManager dollAttackManager;
    private DollHealthManager dollHealthManager;

    public ActiveAttack cprAttack;
    public bool cprActive;
    public bool cprFrozen;

    private bool cprTriggered = false;

    public GameObject deathUI;


    void Start()
    {
        deathUI.SetActive(false);
        dollAttackManager = GetComponent<DollAttackManager>();
        dollHealthManager = GetComponent<DollHealthManager>();

        if (dollAttackManager.cprUI != null)
        {
            dollAttackManager.cprUI.SetActive(false);
        }
    }

    void Update()
    {
        if (!cprTriggered && Data.dollHealth <= cprThreshold && Data.dollHealth > 0)
        {
            StartCPR();
        }

        if (cprActive && !cprFrozen && cprAttack != null)
        {
            RunCPRRing();
        }

        if (Data.dollHealth > cprThreshold)
        {
            cprTriggered = false;
        }


    }

    public void StartCPR()
    {
        if (dollAttackManager.cprUI == null) return;

        cprActive = true;
        cprFrozen = false;
        cprTriggered = true;
        successfulPresses = 0;

        // Stop current attack
        dollAttackManager.isAttacking = false;

        if (dollAttackManager.currentStartAttackCoroutine != null)
        {
            dollAttackManager.StopCoroutine(dollAttackManager.currentStartAttackCoroutine);

            dollAttackManager.currentStartAttackCoroutine = null;
        }

        if (dollAttackManager.currentAttackCoroutine != null)
        {
            dollAttackManager.StopCoroutine(dollAttackManager.currentAttackCoroutine);
            dollAttackManager.currentAttackCoroutine = null;
        }


        if (dollAttackManager.headAttackUI != null)
            dollAttackManager.headAttackUI.SetActive(false);

        if (dollAttackManager.leftArmAttackUI != null)
            dollAttackManager.leftArmAttackUI.SetActive(false);

        if (dollAttackManager.rightArmAttackUI != null)
            dollAttackManager.rightArmAttackUI.SetActive(false);

        if (dollAttackManager.leftLegAttackUI != null)
            dollAttackManager.leftLegAttackUI.SetActive(false);

        if (dollAttackManager.rightLegAttackUI != null)
            dollAttackManager.rightLegAttackUI.SetActive(false);

        dollAttackManager.attack1 = null;
        dollAttackManager.attack2 = null;
        dollAttackManager.attack1Frozen = false;
        dollAttackManager.attack2Frozen = false;

        GameObject ui = dollAttackManager.cprUI;
        ui.SetActive(true);

        GameObject ring = ui.transform.GetChild(0).gameObject;
        Image ringImage = ring.GetComponent<Image>();
        Image circleImage = ui.transform.GetChild(1).GetComponent<Image>();

        ring.transform.localScale = new Vector3(
            dollAttackManager.ringSizeMax,
            dollAttackManager.ringSizeMax,
            dollAttackManager.ringSizeMax
        );

        ringImage.color = Color.white;
        circleImage.color = Color.white;

        cprAttack = new ActiveAttack
        {
            attackObject = ui,
            ringObject = ring,
            ringImage = ringImage,
            circleImage = circleImage,
            attackName = "CPR",
            isGreen = false
        };

        Debug.Log("CPR started");
    }

    void RunCPRRing()
    {
        if (!cprActive || cprAttack == null) return;

        Vector3 scale = cprAttack.ringObject.transform.localScale;
        scale -= Vector3.one * Data.ringSpeed * Time.deltaTime;
        cprAttack.ringObject.transform.localScale = scale;

        if (scale.x <= dollAttackManager.ringCorrectMax)
        {
            cprAttack.isGreen = true;
            cprAttack.ringImage.color = Color.green;
        }

        if (scale.x <= dollAttackManager.ringSizeMin)
        {
            Debug.Log("CPR wrong timing!");
            CPRFail();
        }
    }
    public void CPRSuccess()
    {
        if (!cprActive || cprAttack == null) return;

        successfulPresses++;
        Debug.Log("CPR success " + successfulPresses);

        if (successfulPresses >= pressesNeeded)
        {
            cprActive = false;
            cprFrozen = true;

            dollHealthManager.GainHealth(healAmount);
            Debug.Log("CPR completed, doll healed");

            EndCurrentCPR();
            return;
        }

        ResetCPRRing();
    }

    void ResetCPRRing()
    {
        if (cprAttack == null) return;

        cprFrozen = false;
        cprAttack.isGreen = false;

        cprAttack.ringObject.transform.localScale = new Vector3(
            dollAttackManager.ringSizeMax,
            dollAttackManager.ringSizeMax,
            dollAttackManager.ringSizeMax
        );

        cprAttack.ringImage.color = Color.white;
        cprAttack.circleImage.color = Color.white;
    }



    public void CPRFail()
    {
        if (!cprActive || cprAttack == null) return;

        cprActive = false;
        cprFrozen = true;

        Debug.Log("CPR failed - instant death");
        deathUI.SetActive(true);


        Data.dollHealth = 0;
        EndCurrentCPR();
    }
    public void EndCurrentCPR()
    {
        if (cprAttack != null)
        {
            cprAttack.attackObject.SetActive(false);
            cprAttack.ringImage.color = Color.white;
            cprAttack.circleImage.color = Color.white;
            cprAttack.isGreen = false;
        }

        cprAttack = null;
        cprFrozen = false;
        cprActive = false;
        successfulPresses = 0;
    }

}