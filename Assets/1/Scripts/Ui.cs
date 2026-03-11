using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class Ui : MonoBehaviour
{
    public GameObject HelpUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HelpUI.SetActive(false);
    }

    // Update is called once per frame
    public void NeedHelp()
    {
        HelpUI.SetActive(!HelpUI.activeSelf);

    }
}
