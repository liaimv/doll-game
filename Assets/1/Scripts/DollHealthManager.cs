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

    void Start()
    {
        dollHealthSliderFill = dollHealthSlider.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
        dollHealthSliderFill.color = highHealthCol;
        dollHealthSlider.value = Data.dollHealth;
        deathUI.SetActive(false);
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
        }
        else if (Data.dollHealth < 33)
        {
            dollHealthSliderFill.color = lowHealthCol;
        }
        else if (Data.dollHealth < 66)
        {
            dollHealthSliderFill.color = midHealthCol;
        }
        else
        {
            dollHealthSliderFill.color = highHealthCol;
        }
    }
}

