using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class Speedometer : MonoBehaviour
{
    public FMISimulator FMI;
    public float Vx;

    private ROSConnection ros;
    public string topicName = "/KIMM/speedometer";
    
    // 퍼블리시 주기 (Hz)
    public float publishRateHz = 10.0f; // 1초에 한 번 퍼블리시

    private Stopwatch stopwatch;
    private double PublishPeriodMilliseconds => 1000.0 / publishRateHz;
    private bool ShouldPublishMessage => stopwatch.ElapsedMilliseconds >= PublishPeriodMilliseconds;


    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32Msg>(topicName);
        
        // 스탑워치 초기화 및 시작
        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldPublishMessage)
        {
            stopwatch.Restart();

            Vx=(float)FMI.simulationResult[19];
            Float32Msg speed_msg = new Float32Msg(Vx);

            ros.Publish(topicName, speed_msg);
        }
        
    }
}
