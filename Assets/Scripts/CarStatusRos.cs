using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ROS
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class CarStatusRos : MonoBehaviour
{   
    private ROSConnection ros;
    public string statusCmdTopic = "/KIMM/status/cmd";

    private InputDeviceController vehCmd;

    void Start()
    {   
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(statusCmdTopic);

        vehCmd = transform.GetComponent<InputDeviceController>();
    }

    
    void Update()
    {
        // Debug.Log(vehCmd.steeringInput);
        
        // Vehicle Command Publish
        TwistMsg vehCmdMsg = new TwistMsg();
        vehCmdMsg.linear.x = vehCmd.throttleInput;
        vehCmdMsg.linear.y = vehCmd.steeringInput;
        vehCmdMsg.linear.z = vehCmd.brakeInput;
        vehCmdMsg.angular.x = vehCmd.gearInput;
        ros.Publish(statusCmdTopic, vehCmdMsg);
    }
}
