using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Diagnostics;

public class CarController : MonoBehaviour
{
    Stopwatch sw;
    Stopwatch sw2;

    public Transform parent_coordinate;
    public List<Transform> WheelsTransform;
    public List<Transform> BodyTransform;
    private List<float> rayInfo = new List<float>();
    float[] list_wheel_ray = new float[4] { -0.7354f, -0.7354f, -0.7354f, -0.7354f }; // { 1.05f, 1.05f, 1.05f, 1.05f };
    float[] list_wheel_z = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
    float[] list_wheel_roll = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };

    Vector3 forward_FL = new Vector3(0, 0, 1);
    Vector3 forward_FR = new Vector3(0, 0, 1);
    Vector3 forward_RR = new Vector3(0, 0, 1);
    Vector3 forward_RL = new Vector3(0, 0, 1);

    Vector3 left_FL = new Vector3(-1, 0, 0);
    Vector3 left_FR = new Vector3(-1, 0, 0);
    Vector3 left_RR = new Vector3(-1, 0, 0);
    Vector3 left_RL = new Vector3(-1, 0, 0);

    Quaternion parent_rotation_inv = new Quaternion(0, 0, 0, 1);

    public float left_steer = 0.0f, right_steer = 0.0f;
    readonly float wheelRadius = 0.3677536f;
    float suspension_dist = 0.4177536f;

    public float value_yaw = 0.0f, value_x = 0.0f, value_x_old = 0.0f, value_y = 0.0f, value_y_old = 0.0f, cur_time = 0;
    public float value_roll = 0.0f, value_pitch = 0.0f;
    public static float value_Vz = 0.0f, value_Vx = 0.0f, value_Vy = 0.0f;
    float value_z = 0, value_dz;
    float value_yaw_old = 0;
    float body_fixed_vx = 0, body_fixed_vy = 0;
    int layer_mask;

    // Start is called before the first frame update
    void Start()
    {
        sw = new Stopwatch();
        sw2 = new Stopwatch();
        parent_coordinate.position = new Vector3(-13, 0, -780);
        layer_mask = 1 << LayerMask.NameToLayer("Terrain");
        Transform[] allChildren = GetComponentsInChildren<Transform>();

        WheelsTransform = (from i in allChildren
                           where i.name.Contains("Wheel") // && i.name.Contains("pivot")
                           select i).ToList();

        //WheelsInertialTransform = (from i in allChildren
        //                           where i.name.Contains("Transform")
        //                           select i).ToList();

        BodyTransform = (from i in allChildren
                         where i.name.Contains("Body") // && i.name.Contains("pivot")
                         select i).ToList();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        sw2.Restart();
        transform.GetComponent<FMISimulator>().receiveRayInfo(rayInfo);

        transform.position = new Vector3(transform.position.x, value_z + suspension_dist, transform.position.z);
        transform.rotation = Quaternion.Euler(value_pitch, -value_yaw_old, -value_roll);
        transform.rotation = Quaternion.Euler(value_pitch, -value_yaw, -value_roll);
        transform.position = new Vector3(-value_y, transform.position.y, value_x);
        value_x_old = value_x;
        value_y_old = value_y;
        value_yaw_old = value_yaw;
        foreach (Transform t in WheelsTransform)
        {
            RaycastHit[] wheelHits = Physics.RaycastAll(t.position, -transform.up, wheelRadius + 10.05f, layer_mask);

            if (t.name == "WheelFL")
            {
                //Debug.Log("FrontLeftWheel");
                t.localRotation = Quaternion.Euler(new Vector3(0, -left_steer + 180, 0));
                RaycastHit[] wheelHits_f = Physics.RaycastAll(t.position - t.forward * 0.1f, -transform.up, wheelRadius + 10.0f, layer_mask);
                RaycastHit[] wheelHits_l = Physics.RaycastAll(t.position + t.right * 0.1f, -transform.up, wheelRadius + 10.0f, layer_mask);
                t.localRotation = Quaternion.Euler(new Vector3(-list_wheel_roll[0], -left_steer + 180, 0));

                if (wheelHits.Length > 0 && wheelHits_f.Length > 0 && wheelHits_l.Length > 0)
                {
                    list_wheel_ray[0] = wheelHits[0].point.y - parent_coordinate.position.y;
                    forward_FL = Rotate(parent_rotation_inv, (wheelHits_f[0].point - wheelHits[0].point));
                    left_FL = Rotate(parent_rotation_inv, (wheelHits_l[0].point - wheelHits[0].point));
                }
                else
                {
                    list_wheel_ray[0] = -1000.0f;
                }

                t.localPosition = new Vector3(t.localPosition.x, -suspension_dist - list_wheel_z[0], t.localPosition.z);
            }
            if (t.name == "WheelFR")
            {
                t.localRotation = Quaternion.Euler(new Vector3(0, -right_steer, 0));
                RaycastHit[] wheelHits_f = Physics.RaycastAll(t.position + t.forward * 0.1f, -transform.up, wheelRadius + 10.0f, layer_mask);
                RaycastHit[] wheelHits_l = Physics.RaycastAll(t.position - t.right * 0.1f, -transform.up, wheelRadius + 10.0f, layer_mask);
                t.localRotation = Quaternion.Euler(new Vector3(list_wheel_roll[1], -right_steer, 0));

                if (wheelHits.Length > 0 && wheelHits_f.Length > 0 && wheelHits_l.Length > 0)
                {
                    list_wheel_ray[1] = wheelHits[0].point.y - parent_coordinate.position.y;
                    forward_FR = Rotate(parent_rotation_inv, (wheelHits_f[0].point - wheelHits[0].point));
                    left_FR = Rotate(parent_rotation_inv, (wheelHits_l[0].point - wheelHits[0].point));
                }
                else
                {
                    list_wheel_ray[1] = -1000.0f;
                }

                t.localPosition = new Vector3(t.localPosition.x, -suspension_dist - list_wheel_z[1], t.localPosition.z);
            }
            if (t.name == "WheelRR")
            {
                t.localRotation = Quaternion.Euler(new Vector3(list_wheel_roll[2], 0, 0));
                RaycastHit[] wheelHits_f = Physics.RaycastAll(t.position + transform.forward * 0.1f, -transform.up, wheelRadius + 10.0f, layer_mask);
                RaycastHit[] wheelHits_l = Physics.RaycastAll(t.position - transform.right * 0.1f, -transform.up, wheelRadius + 10.0f, layer_mask);

                if (wheelHits.Length > 0 && wheelHits_f.Length > 0 && wheelHits_l.Length > 0)
                {
                    list_wheel_ray[2] = wheelHits[0].point.y - parent_coordinate.position.y;
                    forward_RR = Rotate(parent_rotation_inv, (wheelHits_f[0].point - wheelHits[0].point));
                    left_RR = Rotate(parent_rotation_inv, (wheelHits_l[0].point - wheelHits[0].point));
                }
                else
                {
                    list_wheel_ray[2] = -1000.0f;
                }

                t.localPosition = new Vector3(t.localPosition.x, -suspension_dist - list_wheel_z[2], t.localPosition.z);
            }
            if (t.name == "WheelRL")
            {
                t.localRotation = Quaternion.Euler(new Vector3(-list_wheel_roll[3], 180, 0));
                RaycastHit[] wheelHits_f = Physics.RaycastAll(t.position + transform.forward * 0.1f, -transform.up, wheelRadius + 10.0f, layer_mask);
                RaycastHit[] wheelHits_l = Physics.RaycastAll(t.position - transform.right * 0.1f, -transform.up, wheelRadius + 10.0f, layer_mask);

                if (wheelHits.Length > 0 && wheelHits_f.Length > 0 && wheelHits_l.Length > 0)
                {
                    list_wheel_ray[3] = wheelHits[0].point.y - parent_coordinate.position.y;
                    forward_RL = Rotate(parent_rotation_inv, (wheelHits_f[0].point - wheelHits[0].point));
                    left_RL = Rotate(parent_rotation_inv, (wheelHits_l[0].point - wheelHits[0].point));
                }
                else
                {
                    list_wheel_ray[3] = -1000.0f;
                }

                t.localPosition = new Vector3(t.localPosition.x, -suspension_dist - list_wheel_z[3], t.localPosition.z);
            }
        }
        sw2.Stop(); // 타이머 중지
        //UnityEngine.Debug.Log($"Transform FixedUpdate 처리 시간: {sw2.Elapsed.TotalMilliseconds:F3} ms"); // 소수점 3자리까지 출력
        
    }

    private Vector3 Rotate(Quaternion q, Vector3 v)
    {
        return q * v; // Unity의 내장 함수 사용
    }

    public void receiveSimulationResult(List<float> simulationResult)
    {
        //UnityEngine.Debug.Log("Simulation 진행 중");
        cur_time = simulationResult[0];
        value_x = simulationResult[1];
        value_y = simulationResult[2];
        value_yaw = simulationResult[3];
        value_z = simulationResult[4];
        value_roll = simulationResult[5];
        value_pitch = simulationResult[6];
        list_wheel_z[0] = simulationResult[7];
        list_wheel_z[1] = simulationResult[8];
        list_wheel_z[2] = simulationResult[9];
        list_wheel_z[3] = simulationResult[10];
        left_steer = simulationResult[11];
        right_steer = simulationResult[12];
        value_Vx = simulationResult[13];
        value_Vy = simulationResult[14];
        list_wheel_roll[0] = simulationResult[15];
        list_wheel_roll[1] = simulationResult[16];
        list_wheel_roll[2] = simulationResult[17];
        list_wheel_roll[3] = simulationResult[18];
        body_fixed_vx = simulationResult[19];
        body_fixed_vy = simulationResult[20];

        rayInfo = new List<float>
        {
            list_wheel_ray[0], list_wheel_ray[1], list_wheel_ray[2], list_wheel_ray[3],
            forward_FL.z, -forward_FL.x, forward_FL.y,
            forward_FR.z, -forward_FR.x, forward_FR.y,
            forward_RR.z, -forward_RR.x, forward_RR.y,
            forward_RL.z, -forward_RL.x, forward_RL.y,
            left_FL.z, -left_FL.x, left_FL.y,
            left_FR.z, -left_FR.x, left_FR.y,
            left_RR.z, -left_RR.x, left_RR.y,
            left_RL.z, -left_RL.x, left_RL.y
        };

        sw.Stop(); // 타이머 중지
        //UnityEngine.Debug.Log($"receiveSimulationResult 실행 시간: {sw.Elapsed.TotalMilliseconds:F3} ms"); // 소수점 3자리까지 출력

        sw.Restart();
    }
}
