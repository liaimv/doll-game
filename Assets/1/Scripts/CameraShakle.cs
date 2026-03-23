using UnityEngine;

public class CameraShakle : MonoBehaviour
{
    public float jitterAmount = 0.05f;

    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.position;
    }

    void LateUpdate()
    {
        Vector3 randomOffset = Random.insideUnitSphere * jitterAmount;
        transform.position = originalPos + randomOffset;
    }
}
