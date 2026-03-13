using UnityEngine;

public class SoulLivesManager : MonoBehaviour
{
    public GameObject soulLivesObject;
    public GameObject friendDeathUIObject;

    private GameObject soulLife1;
    private GameObject soulLife2;
    private GameObject soulLife3;

    private bool soulInHitArea = false;

    private Transform currentHitArea;

    private Transform hitAreaTarget;

    void Start()
    {
        friendDeathUIObject.SetActive(false);

        soulLife1 = soulLivesObject.transform.GetChild(0).gameObject;
        soulLife2 = soulLivesObject.transform.GetChild(1).gameObject;
        soulLife3 = soulLivesObject.transform.GetChild(2).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HitArea")) 
        {
            soulInHitArea = true;

            currentHitArea = other.gameObject.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("HitArea"))
        {
            soulInHitArea = false;
        }
    }

    public void HitSoulCPR()
    {
        if (soulInHitArea)
        {
            if (Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.K))
            {
                HitSoulMediumStrong();
            }
        }
    }

    public void HitSoul(string attackName)
    {
        Debug.Log("soul check happening");
        if (soulInHitArea)
        {
            Debug.Log("soul is in hit area");
            switch (attackName)
            {
                case "Head":
                    hitAreaTarget = GameObject.Find("Head Soul Location").transform;
                    break;

                case "LeftArm":
                    hitAreaTarget = GameObject.Find("L Arm Soul Location").transform;
                    break;

                case "RightArm":
                    hitAreaTarget = GameObject.Find("R Arm Soul Location").transform;
                    break;

                case "LeftLeg":
                    hitAreaTarget = GameObject.Find("L Leg Soul Location").transform;
                    break;

                case "RightLeg":
                    hitAreaTarget = GameObject.Find("R Leg Soul Location").transform;
                    break;
            }

            Vector3 hitAreaPos = hitAreaTarget.position;

            if (hitAreaPos == currentHitArea.position)
            {
                if (Data.isHitMedium || Data.isHitStrong) HitSoulMediumStrong();
            }
        }
    }

    public void HitSoulMediumStrong()
    {
        if (soulLife3.activeSelf)
        {
            soulLife3.SetActive(false);
        }
        else if (soulLife2.activeSelf)
        {
            soulLife2.SetActive(false);
        }
        else if (soulLife1.activeSelf)
        {
            soulLife1.SetActive(false);
            friendDeathUIObject.SetActive(true);
        }
    }
}
