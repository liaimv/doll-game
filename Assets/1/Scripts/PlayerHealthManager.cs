using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public Color highHealthCol;
    public Color midHealthCol;
    public Color lowHealthCol;

    public Slider playerHealthSlider;
    private Image playerHealthSliderFill;
    public GameObject deathUI;

    void Start()
    {
        playerHealthSliderFill = playerHealthSlider.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
        playerHealthSliderFill.color = highHealthCol;
        playerHealthSlider.value = Data.playerHealth;

        deathUI.SetActive(false);

    }

    public void loseHealth(float hitAmount)
    {
        Data.playerHealth -= hitAmount;

        playerHealthSlider.value = Data.playerHealth;

        if (Data.playerHealth < 5)
        {
            playerHealthSliderFill.color = lowHealthCol;
            deathUI.SetActive(true);
        }
        else if (Data.playerHealth < 33)
        {
            playerHealthSliderFill.color = lowHealthCol;
        }
        else if (Data.playerHealth < 66)
        {
            playerHealthSliderFill.color = midHealthCol;
        }
        else
        {
            playerHealthSliderFill.color = highHealthCol;
        }
    }
}
