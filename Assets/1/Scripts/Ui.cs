using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using System.Collections.Generic;


public class Ui : MonoBehaviour
{
    public GameObject HelpUI;

    public DollAttackManager dollAttackManager;
    public Restart restart;

    public List<GameObject> endUIs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //HelpUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.K))
        {
            HelpUI.SetActive(false);
            dollAttackManager.GameStart();

            if (AnyActive())
            {
                restart.TryAgain();
            }
        }
    }

    public bool AnyActive()
    {
        foreach (GameObject endUI in endUIs)
        {
            if (endUI.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    public void NeedHelp()
    {
        HelpUI.SetActive(!HelpUI.activeSelf);

    }
}
