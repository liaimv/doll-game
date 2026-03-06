using UnityEngine;

public class SeeThroughFollowSoul : MonoBehaviour
{
    public static int posID = Shader.PropertyToID("_Soul_Position");
    public Material mat;

    // World space min/max of the soul
    public Vector2 soulWorldPos = new Vector2(419f, 371f);

    // Shader hole min/max
    public Vector2 holePos = new Vector2(359.82f, 318.04f);

    void Update()
    {
        float soulX = transform.position.x;
        float soulY = transform.position.y;

        // Remap X/Y from world space → shader space
        float tX = Mathf.InverseLerp(-soulWorldPos.x, soulWorldPos.x, soulX);
        float tY = Mathf.InverseLerp(-soulWorldPos.y, soulWorldPos.y, soulY);

        float holeX = Mathf.Lerp(-holePos.x, holePos.x, tX);
        float holeY = Mathf.Lerp(-holePos.y, holePos.y, tY);

        mat.SetVector(posID, new Vector2(holeX, holeY));
    }
}
