using UnityEngine;

public class SeeThroughFollowSoul : MonoBehaviour
{
    public static int posID = Shader.PropertyToID("_Soul_Position");

    public Material bodyMat;
    public Material legMat;
    public Material armMat;

    public Material[] headMats;

    public float bodyYThresholdTop;
    public float bodyYThresholdMid = -100f;
    public float bodyYThresholdBottom = -290f;

    public float soulConstantX = 2.4f;
    public float holeConstantX = 2.35f;

    public float bodySoulY = 2.35f;
    public float bodyTopHoleY = 10f;
    public float bodyMidTopHoleY = 10f;
    public float bodyMidHoleY = 10f;
    public float bodyBottomHoleY = 10f;

    public float legSoulY = 2.4f;
    public float legHoleY = 2.35f;

    public float armSoulY = 2.4f;
    public float armHoleY = 2.35f;

    public float headSoulY = 2.4f;
    public float headHoleY = 2.35f;

    void Update()
    {
        float soulX = transform.position.x;
        float soulY = transform.position.y;

        // Remap X/Y from world space → shader space
        float tX = Mathf.InverseLerp(-soulConstantX * 1000, soulConstantX * 1000, soulX);
        float bodytY = Mathf.InverseLerp(-bodySoulY * 1000, bodySoulY * 1000, soulY);
        float legtY = Mathf.InverseLerp(-legSoulY * 1000, legSoulY * 1000, soulY);
        float armtY = Mathf.InverseLerp(-armSoulY * 1000, armSoulY * 1000, soulY);
        float headtY = Mathf.InverseLerp(-headSoulY * 1000, headSoulY * 1000, soulY);

        float holeX = Mathf.Lerp(-holeConstantX * 1000, holeConstantX * 1000, tX);
        float finalBodyTopHoleY = Mathf.Lerp(-bodyTopHoleY * 1000, bodyTopHoleY * 1000, bodytY);
        float finalBodyMidTopHoleY = Mathf.Lerp(-bodyMidTopHoleY * 1000, bodyMidTopHoleY * 1000, bodytY);
        float finalBodyMidHoleY = Mathf.Lerp(-bodyMidHoleY * 1000, bodyMidHoleY * 1000, bodytY);
        float finalBodyBottomHoleY = Mathf.Lerp(-bodyBottomHoleY * 1000, bodyBottomHoleY * 1000, bodytY);
        float finalLegHoleY = Mathf.Lerp(-legHoleY * 1000, legHoleY * 1000, legtY);
        float finalArmHoleY = Mathf.Lerp(-armHoleY * 1000, armHoleY * 1000, armtY);
        float finalHeadHoleY = Mathf.Lerp(-headHoleY * 1000, headHoleY * 1000, headtY);

        //Body different holeY for top, middle, and bottom
        float blendBottom = Mathf.InverseLerp(bodyYThresholdBottom - 200f, bodyYThresholdBottom + 200f, soulY);
        float blendMid = Mathf.InverseLerp(bodyYThresholdMid - 200f, bodyYThresholdMid + 200f, soulY);
        float blendTop = Mathf.InverseLerp(bodyYThresholdTop - 200f, bodyYThresholdTop + 200f, soulY);

        float bodyBottomToMid = Mathf.Lerp(finalBodyBottomHoleY, finalBodyMidHoleY, blendBottom);
        float bodyBottomToMidTop = Mathf.Lerp(finalBodyMidHoleY, finalBodyMidTopHoleY, blendMid);
        float finalBodyHoleY = Mathf.Lerp(bodyBottomToMidTop, finalBodyTopHoleY, blendTop);

        bodyMat.SetVector(posID, new Vector2(holeX, finalBodyHoleY));

        //Legs and arms
        legMat.SetVector(posID, new Vector2(holeX, finalLegHoleY));
        armMat.SetVector(posID, new Vector2(holeX, finalArmHoleY));

        //Head
        Vector2 headHole = new Vector2(holeX, finalHeadHoleY);

        foreach (Material mat in headMats)
        {
            mat.SetVector(posID, headHole);
        }
    }
}
