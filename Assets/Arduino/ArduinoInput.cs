using UnityEngine;

public class ArduinoInput : MonoBehaviour
{
    public SerialController serialController;

    public int head;
    public int leftArm;
    public int rightArm;
    public int leftLeg;
    public int body;

    void Update()
    {
        string message = serialController.ReadSerialMessage();

        if (message == null)
            return;

        string[] values = message.Split(',');

        if(values.Length == 5)
        {
            head = int.Parse(values[0]);
            leftArm = int.Parse(values[1]);
            rightArm = int.Parse(values[2]);
            leftLeg = int.Parse(values[3]);
            body = int.Parse(values[4]);

            Debug.Log(
                "Head:" + head +
                " LArm:" + leftArm +
                " RArm:" + rightArm +
                " LLeg:" + leftLeg +
                " Body:" + body
            );
        }
    }
}