using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryAgain()
    {
        Data.dollHealth = 100;
        Data.playerHealth = 100;

        Data.ringSpeed = Data.ringSlowSpeed;
        Data.soulSpeed = Data.soulSlowSpeed;

        Data.isStage1 = true;
        Data.isStage2 = false;
        Data.isStage3 = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
