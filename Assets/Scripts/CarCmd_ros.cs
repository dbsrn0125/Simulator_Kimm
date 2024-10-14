using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class TwistSubscriber : MonoBehaviour
{
    private ROSConnection ros;
    public string topicName = "/KIMM/cmd";

    public float throttle = 0.0f;
    public float steering_angle = 0.0f;
    public float brake = 0.0f;
    public byte gear = 0;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<TwistMsg>(topicName, TwistMessageReceived);
    }

    private void TwistMessageReceived(TwistMsg msg)
    {
        throttle = (float)msg.linear.x;
        steering_angle = (float)msg.linear.y;
        brake = (float)msg.linear.z;
        gear = (byte)msg.angular.x;
    }
}