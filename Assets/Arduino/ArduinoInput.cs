using UnityEngine;

public class ArduinoInput : MonoBehaviour
{
    public SerialController serialController;

    public int head;
    public int leftArm;
    public int rightArm;
    public int body;

    void Update()
    {
        string message = serialController.ReadSerialMessage();

        if (message == null)
            return;

        string[] values = message.Split(',');

        if(values.Length == 4)
        {
            head = int.Parse(values[0]);
            leftArm = int.Parse(values[1]);
            rightArm = int.Parse(values[2]);
            body = int.Parse(values[3]);

            Debug.Log("Head: " + head +
                      " LArm: " + leftArm +
                      " RArm: " + rightArm +
                      " Body: " + body);
        }
    }
}