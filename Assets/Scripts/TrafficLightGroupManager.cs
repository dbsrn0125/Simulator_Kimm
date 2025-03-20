using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ROS
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
// using RosMessageTypes.TrafficLight;

public enum TrafficLightStatus { Red, Yellow, Green, Left, LeftGreen }

public class TrafficLightGroupManager : MonoBehaviour
{   
    private ROSConnection ros;
    public string trafficLightTopic = "KIMM/status/traffic_light";
    private float publishInterval = 0.2f; // sec

    private List<TrafficLight> trafficLights = new List<TrafficLight>();
    private TrafficLightStatus status;

    float timer = 0.0f;

    void Start()
    {   
        ros = ROSConnection.GetOrCreateInstance();
        // ros.RegisterPublisher<TrafficLightStatusArrayMsg>(trafficLightTopic);

        foreach (Transform child in transform)
        {
            TrafficLight light = child.GetComponent<TrafficLight>();
            if (light != null)
            {
                trafficLights.Add(light);
            }
        }
    }

    void Update()
    {
        StatusUpdate();

        foreach (TrafficLight light in trafficLights)
        {
            light.SetLightStatus(status);
        }

        PublishTrafficLightStatus();
    }
    
    void StatusUpdate()
    {
        timer += Time.deltaTime;

        if (timer < 10f)
        {
            status = TrafficLightStatus.Red;
        }

        else if (timer < 13f)
        {
            status = TrafficLightStatus.Yellow;
        }

        else if (timer < 23f)
        {
           status = TrafficLightStatus.Green;
        }
        else if (timer < 26f)
        {
            status = TrafficLightStatus.Yellow;
        }
        else if (timer < 36f)
        {
            status = TrafficLightStatus.LeftGreen;
        }
        else
        {
            timer = 0.0f;
        }
    }

    void PublishTrafficLightStatus()
    {
        // TrafficLightStatusMsg[] lightStatuses = new TrafficLightStatusMsg[trafficLights.Count];

        // for (int i = 0; i < trafficLights.Count; i++)
        // {
        //     lightStatuses[i] = new TrafficLightStatusMsg
        //     {
        //         id = trafficLights[i].id,
        //         status = (int)trafficLights[i].status
        //     };
        // }

        // // 메시지 생성 및 ROS 퍼블리시
        // TrafficLightStatusArrayMsg msg = new TrafficLightStatusArrayMsg
        // {
        //     header = new HeaderMsg { stamp = new TimeMsg(), frame_id = "map" },
        //     lights = lightStatuses
        // };

        // ros.Publish(trafficLightTopic, msg);
    }
}
