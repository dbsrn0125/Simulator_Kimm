using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class CarController : MonoBehaviour
{
    public Transform parent_coordinate;
    public List<Transform> WheelsTransform;
    //public List<Transform> WheelsInertialTransform;
    public List<Transform> BodyTransform;
    private List<float> rayInfo = new List<float>();
    float[] list_wheel_ray = new float[4] { -0.7354f, -0.7354f, -0.7354f, -0.7354f }; // { 1.05f, 1.05f, 1.05f, 1.05f };
    float[] list_wheel_z = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
    float[] list_wheel_roll = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };

    float[] list_ground_roll = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
    float[] list_ground_pitch = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };

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
    //readonly float wheelRadius = 0.3677536f;
    //float suspension_dist = 0.3677536f;
    //float suspension_dist = 0.4177536f;
    readonly float wheelRadius = 0.3f;
    float suspension_dist = 0.3f;

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
    void Update()
    {
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

            if (t.name == "FrontLeftWheel")
            {
                //Debug.Log("FrontLeftWheel");
                t.localRotation = Quaternion.Euler(new Vector3(0, -left_steer + 180, 0));
                RaycastHit[] wheelHits_f = Physics.RaycastAll(t.position + t.forward * 0.1f, -transform.up, wheelRadius + 10.0f, layer_mask);
                RaycastHit[] wheelHits_l = Physics.RaycastAll(t.position - t.right * 0.1f, -transform.up, wheelRadius + 10.0f, layer_mask);
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

                t.localPosition = new Vector3(t.localPosition.x, suspension_dist - list_wheel_z[0], t.localPosition.z);
            }
            if (t.name == "FrontRightWheel")
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

                t.localPosition = new Vector3(t.localPosition.x, suspension_dist - list_wheel_z[1], t.localPosition.z);
            }
            if (t.name == "RearRightWheel")
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

                t.localPosition = new Vector3(t.localPosition.x, suspension_dist - list_wheel_z[2], t.localPosition.z);
            }
            if (t.name == "RearLeftWheel")
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

                t.localPosition = new Vector3(t.localPosition.x, suspension_dist - list_wheel_z[3], t.localPosition.z);
            }
        }
    }
    private Vector3 Rotate(Quaternion q, Vector3 v)
    {
        Vector3 rotated_v = new Vector3(0, 0, 0);
        rotated_v.x = v.x * (q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z) + 2 * v.y * (q.x * q.y - q.w * q.z) + 2 * v.z * (q.w * q.y + q.x * q.z);
        rotated_v.y = 2 * v.x * (q.w * q.z + q.x * q.y) + v.y * (q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z) + 2 * v.z * (q.y * q.z - q.w * q.x);
        rotated_v.z = 2 * v.x * (q.x * q.z - q.w * q.y) + 2 * v.y * (q.w * q.x + q.y * q.z) + v.z * (q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);
        return rotated_v;
    }

    public void receiveSimulationResult(List<double> simulationResult)
    {
        Debug.Log("receiving SimulationResult");
        cur_time = (float)simulationResult[0];
        value_x = (float)simulationResult[1];
        value_y = (float)simulationResult[2];
        value_yaw = (float)simulationResult[3];
        value_z = (float)simulationResult[4];
        value_roll = (float)simulationResult[5];
        value_pitch = (float)simulationResult[6];
        list_wheel_z[0] = (float)simulationResult[7];
        list_wheel_z[1] = (float)simulationResult[8];
        list_wheel_z[2] = (float)simulationResult[9];
        list_wheel_z[3] = (float)simulationResult[10];
        left_steer = (float)simulationResult[11];
        right_steer = (float)simulationResult[12];
        value_Vx = (float)simulationResult[13];
        value_Vy = (float)simulationResult[14];
        list_wheel_roll[0] = (float)simulationResult[15];
        list_wheel_roll[1] = (float)simulationResult[16];
        list_wheel_roll[2] = (float)simulationResult[17];
        list_wheel_roll[3] = (float)simulationResult[18];
        body_fixed_vx = (float)simulationResult[19];
        body_fixed_vy = (float)simulationResult[20];

        rayInfo.Add(list_wheel_ray[0]);
        rayInfo.Add(list_wheel_ray[1]);
        rayInfo.Add(list_wheel_ray[2]);
        rayInfo.Add(list_wheel_ray[3]);
        rayInfo.Add(forward_FL.z);
        rayInfo.Add(-forward_FL.x);
        rayInfo.Add(forward_FL.y);
        rayInfo.Add(forward_FR.z);
        rayInfo.Add(-forward_FR.x);
        rayInfo.Add(forward_FR.y);
        rayInfo.Add(forward_RR.z);
        rayInfo.Add(-forward_RR.x);
        rayInfo.Add(forward_RR.y);
        rayInfo.Add(forward_RL.z);
        rayInfo.Add(-forward_RL.x);
        rayInfo.Add(forward_RL.y);
        rayInfo.Add(left_FL.z);
        rayInfo.Add(-left_FL.x);
        rayInfo.Add(left_FL.y);
        rayInfo.Add(left_FR.z);
        rayInfo.Add(-left_FR.x);
        rayInfo.Add(left_FR.y);
        rayInfo.Add(left_RR.z);
        rayInfo.Add(-left_RR.x);
        rayInfo.Add(left_RR.y);
        rayInfo.Add(left_RL.z);
        rayInfo.Add(-left_RL.x);
        rayInfo.Add(left_RL.y);
        transform.GetComponent<FMISimulator>().receiveRayInfo(rayInfo);
        rayInfo.Clear();
    }
}
