using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;
using System.Collections.Generic;

public class Lidar2DMsgPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "scan";

    public Lidar2DSacnner lidar;
    public string frameId = "laser";

    [Tooltip("Publish rate in Hz")]
    public float publishRate = 10f;
    private float timeElapsed;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<LaserScanMsg>(topicName);
        timeElapsed = 0;

        lidar = GetComponent<Lidar2DSacnner>();
        if (lidar == null)
        {
            Debug.LogError("Lidar2D component not found on the same GameObject!");
        }
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= 1f / publishRate)
        {
            PublishScan();
            timeElapsed = 0f;
        }
    }

    void PublishScan()
    {
        if (lidar == null || lidar.Distances == null)
            return;

        List<float> ranges = lidar.Distances;

        int rayCount = ranges.Count;
        float angleMin = 0f;
        float angleMax = 2 * Mathf.PI;
        float angleIncrement = (angleMax - angleMin) / rayCount;

        float rangeMin = 0.01f;
        float rangeMax = lidar.scanRange;

        // Header
        HeaderMsg header = new HeaderMsg();
        header.frame_id = frameId;
        header.stamp = TimeStamp();

        // LaserScan 메시지 생성
        LaserScanMsg msg = new LaserScanMsg
        {
            header = header,
            angle_min = angleMin,
            angle_max = angleMax,
            angle_increment = angleIncrement,
            time_increment = 0, // 고정형 LiDAR이면 생략
            scan_time = 1f / publishRate,
            range_min = rangeMin,
            range_max = rangeMax,
            ranges = ranges.ToArray(),
            intensities = new float[rayCount] // 생략해도 되지만 초기화
        };

        ros.Publish(topicName, msg);
    }

    // ROS 시간 형식으로 변환
    static TimeMsg TimeStamp()
    {
        double t = Time.realtimeSinceStartup;

        // ROS1
        uint secs = (uint)t;
        uint nsecs = (uint)((t - secs) * 1e9);

        // ROS2
        // int secs = (int)t;
        // uint nsecs = (uint)((t - secs) * 1e9);
        
        return new TimeMsg(secs, nsecs);
    }
}