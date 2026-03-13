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
            UpdateRescueUI();
        }
    }

    void StartRescue()
    {
        rescueActive = true;
        successfulPresses = 0;
        comboHeldLastFrame = false;

        if (soulMovement != null)
        {
            soulMovement.enabled = false;
        }

        if (rescueText != null)
        {
            rescueText.gameObject.SetActive(true);
        }

        Debug.Log("rescuera start");
    }

    void MoveSoulToCenterAndShake()
    {
        if (soulTransform == null || centerPoint == null) return;

        soulTransform.position = Vector3.MoveTowards(
            soulTransform.position,
            centerPoint.position,
            moveToCenterSpeed * Time.deltaTime
        );

        if (Vector3.Distance(soulTransform.position, centerPoint.position) < 0.05f)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeAmount;
            shakeOffset.z = 0f; 

            soulTransform.position = centerPoint.position + shakeOffset;
        }
    }

    void HandleComboInput()
    {
        bool allHeld =
            Input.GetKey(KeyCode.X) &&
            Input.GetKey(KeyCode.Z) &&
            Input.GetKey(KeyCode.W) &&
            Input.GetKey(KeyCode.A) &&
            Input.GetKey(KeyCode.D);

        // Count once when the full combo becomes newly pressed
        if (allHeld && !comboHeldLastFrame)
        {
            successfulPresses++;
            Debug.Log("SOUL SAVE HIT: " + successfulPresses + "/" + pressesNeeded);

            if (successfulPresses >= pressesNeeded)
            {
                SaveSoul();
            }
        }

        comboHeldLastFrame = allHeld;
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
    }

    public void ResetCombo()
    {
        if (!rescueActive) return;

        successfulPresses = 0;
        comboHeldLastFrame = false;

        Debug.Log("SOUL RESCUE RESET");
    }
}
