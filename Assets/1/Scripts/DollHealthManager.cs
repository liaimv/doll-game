using UnityEngine;
using UnityEngine.UI;

public class DollHealthManager : MonoBehaviour
{
    public Color highHealthCol;
    public Color midHealthCol;
    public Color lowHealthCol;

    public Slider dollHealthSlider;
    private Image dollHealthSliderFill;

    void Start()
    {
        dollHealthSliderFill = dollHealthSlider.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
        dollHealthSliderFill.color = highHealthCol;
        dollHealthSlider.value = Data.dollHealth;
    }

    public void loseHealth(float hitAmount)
    {
        Data.dollHealth -= hitAmount;

        dollHealthSlider.value = Data.dollHealth;

        if (Data.dollHealth < 33)
        {
            dollHealthSliderFill.color = lowHealthCol;
        }
        else if (Data.dollHealth < 66)
        {
            dollHealthSliderFill.color = midHealthCol;
        }
    }
}
