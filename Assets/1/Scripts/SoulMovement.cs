using UnityEngine;

public class SoulMovement : MonoBehaviour
{
    public Transform StartPos;
    public Transform[] BodyParts;

    public float speed = 2f;

    private Vector3 destination;

    private bool goingToParts = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = StartPos.position;
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        speed += 2f;
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);


        if (Vector3.Distance (transform.position, destination) < 0.05f)
        {
            if (goingToParts)
            {
                destination = StartPos.position;
                goingToParts = false;
            }
            else
            {
                Move();
            }
        }
    }

    void Move()
    {
        int randomPos = Random.Range(0, BodyParts.Length);

        destination = BodyParts[randomPos].position;

        goingToParts = true;
    }
}
