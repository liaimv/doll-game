using UnityEngine;
using UnityEngine.UI;

public class CPR : MonoBehaviour
{
    public float cprThreshold = 20f;
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
        dollAttackManager = GetComponent<DollAttackManager>();
        dollHealthManager = GetComponent<DollHealthManager>();

        if (deathUI != null)
            deathUI.SetActive(false);

        if (dollAttackManager != null && dollAttackManager.cprUI != null)
        {
            dollAttackManager.cprUI.SetActive(false);
        }
    }

    void Update()
    {
        if (!cprTriggered && !cprActive && Data.dollHealth <= cprThreshold && Data.dollHealth > 0)
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
        if (dollAttackManager == null || dollAttackManager.cprUI == null) return;
        if (Data.dollHealth <= 0) return;

        cprActive = true;
        cprFrozen = false;
        cprTriggered = true;
        successfulPresses = 0;

        // FULLY stop all current attacks
        dollAttackManager.StopAttacksForCPR();

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

        // ONE MISS = INSTANT DEATH
        if (scale.x <= dollAttackManager.ringSizeMin)
        {
            Debug.Log("CPR failed: missed even once");
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
            dollAttackManager.ResumeAttacksAfterCPR();
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
        Data.dollHealth = 0;

        if (dollHealthManager != null)
        {
            dollHealthManager.loseHealth();
        }

        deathUI.SetActive(true);
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