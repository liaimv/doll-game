using UnityEngine;

public class SoulMovement : MonoBehaviour
{
    public Transform StartPos;
    public Transform[] BodyParts;

    public float speed = 2f;

    private Vector3 destination;

    private bool goingToParts = true;

    private Transform attackTarget;
    private bool movingToCenter = false;
    private bool movingToAttack = false;
    private bool isRandom = true;

    private Transform previousPart;
    private Transform currentPart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = StartPos.position;
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRandom)
        {
            //speed += 2f;
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime * 100);


            if (Vector3.Distance(transform.position, destination) < 0.05f)
            {
                if (goingToParts)
                {
                    destination = StartPos.position;
                    previousPart = currentPart;
                    goingToParts = false;
                }
                else
                {
                    Move();
                }
            }
        }

        if (movingToCenter)
        {
            transform.position = Vector3.MoveTowards(transform.position, StartPos.position, speed * Time.deltaTime * 100);

            if (Vector3.Distance(transform.position, StartPos.position) < 0.05f)
            {
                movingToCenter = false;
                movingToAttack = true;
                destination = attackTarget.position;
            }
        }

        if (movingToAttack)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime * 100);

            if (Vector3.Distance(transform.position, destination) < 0.05f)
            {
                movingToAttack = false;
            }
        }
    }

    void Move()
    {
        int randomPos = Random.Range(0, BodyParts.Length);

        currentPart = BodyParts[randomPos];
        destination = currentPart.position;

        goingToParts = true;
    }

    public void SoulMoveToAttack(string attackName)
    {
        isRandom = false;

        movingToCenter = false;
        movingToAttack = false;

        switch (attackName)
        {
            case "Head":
                attackTarget = GameObject.Find("Head Soul Location").transform;
                break;

            case "LeftArm":
                attackTarget = GameObject.Find("L Arm Soul Location").transform;
                break;

            case "RightArm":
                attackTarget = GameObject.Find("R Arm Soul Location").transform;
                break;

            case "LeftLeg":
                attackTarget = GameObject.Find("L Leg Soul Location").transform;
                break;

            case "RightLeg":
                attackTarget = GameObject.Find("R Leg Soul Location").transform;
                break;
        }

        Vector3 attackPos = attackTarget.position;

        if (goingToParts && currentPart != null && currentPart.position == attackPos)
        {
            movingToAttack = true;
            return;
        }

        if (!goingToParts && previousPart != null && previousPart.position == attackPos)
        {
            movingToAttack = true;
            destination = attackPos;
            return;
        }


        movingToCenter = true;
    }

    public void SoulRandomMovement()
    {
        goingToParts = false;
        destination = StartPos.position;
        isRandom = true;
    }
}
