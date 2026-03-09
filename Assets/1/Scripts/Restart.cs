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
        Data.friendHealth = 3;
        Data.playerHealth = 100;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
