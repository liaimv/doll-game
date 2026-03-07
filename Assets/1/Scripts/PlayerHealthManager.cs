using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public Color highHealthCol;
    public Color midHealthCol;
    public Color lowHealthCol;

    public Slider playerHealthSlider;
    private Image playerHealthSliderFill;

    void Start()
    {
        playerHealthSliderFill = playerHealthSlider.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
        playerHealthSliderFill.color = highHealthCol;
        playerHealthSlider.value = Data.playerHealth;
    }

    public void loseHealth(float hitAmount)
    {
        Data.playerHealth -= hitAmount;

        playerHealthSlider.value = Data.playerHealth;

        if (Data.playerHealth < 33)
        {
            playerHealthSliderFill.color = lowHealthCol;
        }
        else if (Data.playerHealth < 66)
        {
            playerHealthSliderFill.color = midHealthCol;
        }
    }
}
