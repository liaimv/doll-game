using UnityEngine;

public class ArduinoInput : MonoBehaviour
{
    public SerialController serialController;

    public int head;
    public int leftArm;
    public int rightArm;
    public int leftLeg;
    public int rightLeg;
    public int body;

    void Update()
    {
        string message = serialController.ReadSerialMessage();

        if (message == null)
            return;

        // Ardity系统消息过滤
        if (message == SerialController.SERIAL_DEVICE_CONNECTED)
            return;

        if (message == SerialController.SERIAL_DEVICE_DISCONNECTED)
            return;

        string[] values = message.Split(',');

        if(values.Length == 6)
        {
            head = int.Parse(values[0]);
            leftArm = int.Parse(values[1]);
            rightArm = int.Parse(values[2]);
            leftLeg = int.Parse(values[3]);
            rightLeg = int.Parse(values[4]);
            body = int.Parse(values[5]);

            Debug.Log(
                "Head:" + head +
                " LArm:" + leftArm +
                " RArm:" + rightArm +
                " LLeg:" + leftLeg +
                " RLeg:" + rightLeg +
                " Body:" + body
            );
        }
    }
}