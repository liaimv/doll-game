using UnityEngine;
using TMPro;


public class SoulExtract : MonoBehaviour
{
    public float rescueThreshold = 10f;
    public int pressesNeeded = 8;
    public float healAmount = 20f;

    public Transform soulTransform;
    public Transform centerPoint;
    public SoulMovement soulMovement;

    public float moveToCenterSpeed = 10f;
    public float shakeAmount = 0.12f;

    public TextMeshProUGUI rescueText;

    public DollHealthManager healthManager;

    public bool rescueActive = false;
    public bool dollTrulyDead = false;

    private int successfulPresses = 0;
    private bool comboHeldLastFrame = false;

    public GameObject deathUI;

    public GameObject rescueUI;
    public float upwardStep = 0.3f;

    public DollAttackManager dollAttackManager;

    private bool soulCentered = false;
    private Vector3 rescueBasePosition;
    private float currentRise = 0f;

    public GameObject dollObject;
    public GameObject successObject;

    void Start()
    {
        if (rescueText != null)
        {
            rescueText.gameObject.SetActive(false);
        }

        deathUI.SetActive(false);
        rescueUI.SetActive(false);
    }

    void Update()
    {
        if (dollTrulyDead) return;

        if (!rescueActive && Data.dollHealth > 0 && Data.dollHealth <= rescueThreshold)
        {
            StartRescue();
        }

        if (rescueActive)
        {
        
            //if (Data.dollHealth <= 0)
            //{
            //    FailRescue();
            //    return;
            //}

            MoveSoulToCenterAndShake();
            HandleComboInput();
            //UpdateRescueUI();
        }
    }

    void StartRescue()
    {
        rescueActive = true;
        successfulPresses = 0;
        comboHeldLastFrame = false;

        soulCentered = true;
        currentRise = 0f;

        if (soulMovement != null)
        {
            soulMovement.enabled = false;
        }

        if (soulTransform != null && centerPoint != null)
        {
            rescueBasePosition = centerPoint.position;
            soulTransform.position = rescueBasePosition;
        }

        if (rescueText != null)
        {
            rescueText.gameObject.SetActive(true);
        }

        Debug.Log("rescuera start");

        if (dollAttackManager != null)
        {
            dollAttackManager.StopAttacksForCPR();
            dollAttackManager.StartRescuePulse();
        }
    }

    void MoveSoulToCenterAndShake()
    {
        if (soulTransform == null || centerPoint == null || !soulCentered) return;

        Vector3 shakeOffset = Random.insideUnitSphere * shakeAmount;
        shakeOffset.z = 0f;

        soulTransform.position = rescueBasePosition + Vector3.up * currentRise + shakeOffset;
    }

    void HandleComboInput()
    {
        bool head =
        Input.GetKey(KeyCode.W) ||
        Input.GetKey(KeyCode.T) ||
        Input.GetKey(KeyCode.I);

        bool leftArm =
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.F) ||
            Input.GetKey(KeyCode.J);

        bool rightArm =
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.H) ||
            Input.GetKey(KeyCode.L);

        bool leftLeg =
            Input.GetKey(KeyCode.Z) ||
            Input.GetKey(KeyCode.V) ||
            Input.GetKey(KeyCode.M);

        bool rightLeg =
            Input.GetKey(KeyCode.X) ||
            Input.GetKey(KeyCode.B) ||
            Input.GetKey(KeyCode.Comma);

        bool allPressed = head && leftArm && rightArm && leftLeg && rightLeg;

        if (allPressed && !comboHeldLastFrame)
        {
            successfulPresses++;
            currentRise += upwardStep;

            Debug.Log("SOUL SAVE HIT: " + successfulPresses + "/" + pressesNeeded);

            if (successfulPresses >= pressesNeeded)
            {
                SaveSoul();
            }
        }

        comboHeldLastFrame = allPressed;

        //bool pressedAny =
        //    Input.GetKeyDown(KeyCode.W) ||
        //    Input.GetKeyDown(KeyCode.A) ||
        //    Input.GetKeyDown(KeyCode.D) ||
        //    Input.GetKeyDown(KeyCode.Z) ||
        //    Input.GetKeyDown(KeyCode.X);

        //if (pressedAny)
        //{
        //    successfulPresses++;
        //    currentRise += upwardStep;

        //    Debug.Log("SOUL SAVE HIT: " + successfulPresses + "/" + pressesNeeded);

        //    if (successfulPresses >= pressesNeeded)
        //    {
        //        SaveSoul();
        //    }
        //}
    }

    void UpdateRescueUI()
    {
        if (rescueText == null) return;

        rescueText.text = "SAVE YOUR FRIEND!\n"
                        + successfulPresses + " / " + pressesNeeded;
    }

    void SaveSoul()
    {
        rescueActive = false;
        successfulPresses = 0;
        comboHeldLastFrame = false;

        if (healthManager != null)
        {
            healthManager.GainHealth(healAmount);
        }
        else
        {
            Data.dollHealth += healAmount;
            Data.dollHealth = Mathf.Clamp(Data.dollHealth, 0, 100);
        }

        if (soulMovement != null)
        {
            soulMovement.enabled = true;
            soulMovement.SoulRandomMovement();
        }

        if (rescueText != null)
        {
            rescueText.gameObject.SetActive(false);
        }


        rescueUI.SetActive(true);
        dollObject.SetActive(false);
        successObject.SetActive(true);
        Debug.Log("SOUL SAVED");
    }

    void FailRescue()
    {
        rescueActive = false;
        dollTrulyDead = true;

        if (rescueText != null)
        {
            rescueText.gameObject.SetActive(false);
        }

        deathUI.SetActive(true);

        Debug.Log("DOLL TRULY DEAD");

        if (dollAttackManager != null)
        {
            dollAttackManager.StopRescuePulse();
        }
    }

    public void ResetCombo()
    {
        if (!rescueActive) return;

        successfulPresses = 0;
        comboHeldLastFrame = false;

        Debug.Log("SOUL RESCUE RESET");
    }
}
