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

    void Start()
    {
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

        successfulPresses = 0;

        Debug.Log("CPR started");
    }

    void RunCPRRing()
    {
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
            cprFrozen = true;
        }
    }

    public void CPRSuccess()
    {
        successfulPresses++;
        Debug.Log("CPR success " + successfulPresses);

        if (successfulPresses >= pressesNeeded)
        {
            dollHealthManager.GainHealth(healAmount);
            EndCurrentCPR();
            Debug.Log("CPR completed, doll healed");
        }
        else
        {
            ResetCPRRing();
        }
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