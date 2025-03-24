using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ROS
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.TrafficLight;

public enum TrafficLightStatus { Red, Yellow, Green, Left, LeftGreen }

public class TrafficLightGroupManager : MonoBehaviour
{   
    // ROS
    private ROSConnection ros;
    public string trafficLightTopic = "/KIMM/status/traffic_light";

    public float frequency = 1.0f;

    private Stopwatch stopwatch;
    private double PublishPeriodMilliseconds => 1000.0 / frequency;
    private bool ShouldPublishMessage => stopwatch.ElapsedMilliseconds >= PublishPeriodMilliseconds;

    // Traffic Groups
    private List<TrafficLightGroup> trafficLightGroups = new List<TrafficLightGroup>();

    void Start()
    {   
        // ROS
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseArrayMsg>(trafficLightTopic);
        
        // ros.RegisterPublisher<TrafficLightArrayMsg>(trafficLightTopic);


        // Stopwatch
        stopwatch = new Stopwatch();
        stopwatch.Start();

        foreach (Transform child in transform)
        {
            TrafficLightGroup tl_group = child.GetComponent<TrafficLightGroup>();
            if (tl_group != null)
            {
                trafficLightGroups.Add(tl_group);
            }
        }
    }

    void Update()
    {   
        if (ShouldPublishMessage)
        {
            stopwatch.Restart();

            PublishTrafficLightStatus();
        }
    }

    void PublishTrafficLightStatus() 
    {   
        PoseArrayMsg poseArrayMsg = new PoseArrayMsg();
        List<PoseMsg> poseList = new List<PoseMsg>();

        // TrafficLightArrayMsg msg = new TrafficLightArrayMsg();
        // List<TrafficLightMsg> lightList = new List<TrafficLightMsg>();

        foreach (TrafficLightGroup group in trafficLightGroups)
        {
            foreach (TrafficLight light in group.GetTrafficLights())
            {   
                PoseMsg pose = new PoseMsg();
            
                // 기본 헤더 설정
                // HeaderMsg header = new HeaderMsg();
                // header.frame_id = "map"; // 필요시 "odom" 등으로 변경
                // float time = Time.time;
                // uint sec = (uint)Mathf.FloorToInt(time);
                // uint nanosec = (uint)((time - sec) * 1e9f);

                // header.stamp = new TimeMsg(sec, nanosec);
                
                // pose.header = header;

                // Traffic Light ID를 position.x에, Status를 position.y에 설정
                pose.position = new PointMsg((double)light.id, (double)light.GetStatus(), 0.0);


                // orientation을 기본값 (회전 없음)으로 설정
                pose.orientation = new QuaternionMsg(0, 0, 0, 1);

                poseList.Add(pose);

                // // UnityEngine.Debug.Log($"TrafficLight ID: {light.id}, Status: {light.GetStatus()}");
                // TrafficLightMsg lightMsg = new TrafficLightMsg();
                // lightMsg.id = light.id;
                // lightMsg.status = (int)light.GetStatus();
                // lightList.Add(lightMsg);
            }
        }

        poseArrayMsg.poses = poseList.ToArray();
        ros.Publish(trafficLightTopic, poseArrayMsg);

        // msg.lights = lightList.ToArray();
        // UnityEngine.Debug.Log(msg.lights[0]);

        // ros.Publish(trafficLightTopic, msg);
    }

}
