using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;
using System.Collections.Generic;

public class Lidar2DSacnner : MonoBehaviour
{
    [Header("LiDAR Settings")]
    public int numberOfRays = 360;          // 몇 개의 레이 발사
    public float scanRange = 10f;           // 최대 감지 거리
    public float scanFrequency = 10f;       // 스캔 주기 (Hz)
    public LayerMask detectionMask;         // 감지할 레이어

    private float scanTimer;
    private float scanInterval;
    private List<float> distances;          // 결과 저장

    public List<float> Distances => distances;

    // ROS
    ROSConnection ros;
    public string topicName = "scan";
    public string frameId = "laser";

    [Tooltip("Publish rate in Hz")]
    public float publishRate = 10f;
    private float timeElapsed;

    void Start()
    {
        scanInterval = 1f / scanFrequency;
        distances = new List<float>(new float[numberOfRays]);

        // ROS
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<LaserScanMsg>(topicName);
        timeElapsed = 0;
    }

    void Update()
    {   
        // Scan
        scanTimer += Time.deltaTime;
        if (scanTimer >= scanInterval)
        {
            PerformScan();
            scanTimer = 0f;
        }

        // Publish
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= 1f / publishRate)
        {
            PublishScan();
            timeElapsed = 0f;
        }

    }

    void PerformScan()
    {
        float angleIncrement = 360f / numberOfRays;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = angleIncrement * i;
            Vector3 direction = AngleToDirection(angle);
            
            // Vector3 direction = new Vector3(direction2D.x, 0f, direction2D.y);  // 2D 방향을 3D로 변환
            direction = transform.rotation * direction; // 회전 반영
            
            // x, z 값만 사용하여 Vector3로 설정
            Vector3 origin = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            UnityEngine.Debug.Log(origin);
            // Debug.DrawRay(origin, direction * scanRange, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, scanRange, detectionMask))
            {
                distances[i] = hit.distance;
            }
            else
            {
                distances[i] = float.PositiveInfinity;
            }
        }
    }

    Vector3 AngleToDirection(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector3(-Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }

    void OnDrawGizmos()
    {
        if (distances == null || distances.Count != numberOfRays)
            return;

        Gizmos.color = Color.green;
        float angleIncrement = 360f / numberOfRays;
        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = angleIncrement * i;
            Vector2 direction = AngleToDirection(angle);
            Vector2 origin = transform.position;
            Vector2 endpoint = origin + direction * distances[i];

            Gizmos.DrawLine(origin, endpoint);
        }
    }

    void PublishScan()
    {
        List<float> ranges = distances;

        int rayCount = ranges.Count;
        float angleMin = 0f;
        float angleMax = 2 * Mathf.PI;
        float angleIncrement = (angleMax - angleMin) / rayCount;

        float rangeMin = 0.01f;
        float rangeMax = scanRange;

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
            // intensities = new float[rayCount] // 생략해도 되지만 초기화
        };

        ros.Publish(topicName, msg);
    }

    // ROS 시간 형식으로 변환
    static TimeMsg TimeStamp()
    {
        double t = Time.realtimeSinceStartup;
        uint secs = (uint)t;
        uint nsecs = (uint)((t - secs) * 1e9);
        return new TimeMsg(secs, nsecs);
    }

}