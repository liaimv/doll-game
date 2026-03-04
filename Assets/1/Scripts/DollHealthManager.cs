using UnityEngine;
using UnityEngine.UI;

public class DollHealthManager : MonoBehaviour
{
    public Slider dollHealthSlider;

    void Start()
    {
        dollHealthSlider.value = Data.dollHealth;
    }

    void Update()
    {
    }

    public void loseHealth(float hitAmount)
    {
        Data.dollHealth -= hitAmount;

        dollHealthSlider.value = Data.dollHealth;
    }
}
