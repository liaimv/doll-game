using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    public int attackAmount = 10;
    public bool isBeingAttacked = false;

    private DollHealthManager dollHealthManager;

    void Start()
    {
        dollHealthManager = GetComponent<DollHealthManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) //Head
        {
            DollGotHit();
        }
        else if (Input.GetKeyDown(KeyCode.S)) //Body
        {
            DollGotHit();
        }
        else if (Input.GetKeyDown(KeyCode.A)) //Left Arm
        {
            DollGotHit();
        }
        else if (Input.GetKeyDown(KeyCode.D)) //Right Arm
        {
            DollGotHit();
        }
        else if (Input.GetKeyDown(KeyCode.Z)) //Left Leg
        {
            DollGotHit();
        }
        else if (Input.GetKeyDown(KeyCode.X)) //Right Leg
        {
            DollGotHit();
        }
    }

    void DollGotHit()
    {
        dollHealthManager.loseHealth(attackAmount);
    }
}
