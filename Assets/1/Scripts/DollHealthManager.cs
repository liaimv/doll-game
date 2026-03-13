using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DollHealthManager : MonoBehaviour
{
    public Color highHealthCol;
    public Color midHealthCol;
    public Color lowHealthCol;

    public Slider dollHealthSlider;
    private Image dollHealthSliderFill;
    public GameObject deathUI;

    public Animator[] animators;

    void Start()
    {
        dollHealthSliderFill = dollHealthSlider.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
        dollHealthSliderFill.color = highHealthCol;
        dollHealthSlider.value = Data.dollHealth;
        deathUI.SetActive(false);

        foreach (Animator animator in animators)
        {
            animator.speed = Data.animationSlowSpeed;
        }
    }

    public void loseHealth(float hitAmount)
    {
        Data.dollHealth -= hitAmount;
        Data.dollHealth = Mathf.Clamp(Data.dollHealth, 0, 100);

        loseHealth();
    }
    public void GainHealth(float healAmount)
    {
        Data.dollHealth += healAmount;
        Data.dollHealth = Mathf.Clamp(Data.dollHealth, 0, 100);

        loseHealth();
    }
    public void loseHealth()
    {
        dollHealthSlider.value = Data.dollHealth;

        if (Data.dollHealth <= 0)
        {
            dollHealthSliderFill.color = lowHealthCol;
            deathUI.SetActive(true);
            return;
        }
        else if (Data.dollHealth < 33)
        {
            dollHealthSliderFill.color = lowHealthCol;
            Data.isStage3 = true;
        }
        else if (Data.dollHealth < 66)
        {
            dollHealthSliderFill.color = midHealthCol;
            Data.isStage2 = true;
        }
        else
        {
            dollHealthSliderFill.color = highHealthCol;
        }

        if (Data.isStage3)
        {
            Data.ringSpeed = Data.ringFastSpeed;
            Data.soulSpeed = Data.soulFastSpeed;

            foreach (Animator animator in animators)
            {
                animator.speed = Data.animationFastSpeed;
            }
        }
        else if (Data.isStage2)
        {
            Data.ringSpeed = Data.ringMediumSpeed;
            Data.soulSpeed = Data.soulMediumSpeed;

            foreach (Animator animator in animators)
            {
                animator.speed = Data.animationMediumSpeed;
            }
        }
        else if (Data.isStage1)
        {
            Data.ringSpeed = Data.ringSlowSpeed;
            Data.soulSpeed = Data.soulSlowSpeed;

            foreach (Animator animator in animators)
            {
                animator.speed = Data.animationSlowSpeed;
            }
        }
    }
}

