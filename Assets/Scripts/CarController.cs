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
    public Vector3 initial_position;
    public Quaternion initial_rotation;
    public List<Transform> WheelsTransform;
    public List<Transform> BodyTransform;
    public FMISimulator FMI;
    private float[] rayInfo = new float[28];
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
    public static event Action OnCarCollision;
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
    bool collision;
    // Start is called before the first frame update
    void Start()
    {
        FMI = transform.GetComponent<FMISimulator>();
        initial_position = new Vector3(0, 0, 0);
        sw = new Stopwatch();
        sw2 = new Stopwatch();
        parent_coordinate.position = new Vector3(-13, 0, -780);
        layer_mask = 1 << LayerMask.NameToLayer("Map");
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

    private void Update()
    {
        if (!collision)
        {
            FMI.receiveRayInfo(rayInfo);
            transform.position = new Vector3(transform.position.x, value_z + suspension_dist, transform.position.z);
            transform.rotation = Quaternion.Euler(value_pitch, -value_yaw_old, -value_roll);
            transform.rotation = Quaternion.Euler(value_pitch, -value_yaw, -value_roll);
            transform.position = initial_position + new Vector3(-value_y, transform.position.y, value_x);

        }

        value_x_old = value_x;
        value_y_old = value_y;
        value_yaw_old = value_yaw;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        sw2.Restart();
        
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

    public void receiveSimulationResult(float[] simulationResult)
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

        rayInfo[0] = list_wheel_ray[0];
        rayInfo[1] = list_wheel_ray[1];
        rayInfo[2] = list_wheel_ray[2];
        rayInfo[3] = list_wheel_ray[3];

        rayInfo[4] = forward_FL.z;
        rayInfo[5] = -forward_FL.x;
        rayInfo[6] = forward_FL.y;

        rayInfo[7] = forward_FR.z;
        rayInfo[8] = -forward_FR.x;
        rayInfo[9] = forward_FR.y;

        rayInfo[10] = forward_RR.z;
        rayInfo[11] = -forward_RR.x;
        rayInfo[12] = forward_RR.y;

        rayInfo[13] = forward_RL.z;
        rayInfo[14] = -forward_RL.x;
        rayInfo[15] = forward_RL.y;

        rayInfo[16] = left_FL.z;
        rayInfo[17] = -left_FL.x;
        rayInfo[18] = left_FL.y;

        rayInfo[19] = left_FR.z;
        rayInfo[20] = -left_FR.x;
        rayInfo[21] = left_FR.y;

        rayInfo[22] = left_RR.z;
        rayInfo[23] = -left_RR.x;
        rayInfo[24] = left_RR.y;

        rayInfo[25] = left_RL.z;
        rayInfo[26] = -left_RL.x;
        rayInfo[27] = left_RL.y;

        //rayInfo = new List<float>
        //{
        //    list_wheel_ray[0], list_wheel_ray[1], list_wheel_ray[2], list_wheel_ray[3],
        //    forward_FL.z, -forward_FL.x, forward_FL.y,
        //    forward_FR.z, -forward_FR.x, forward_FR.y,
        //    forward_RR.z, -forward_RR.x, forward_RR.y,
        //    forward_RL.z, -forward_RL.x, forward_RL.y,
        //    left_FL.z, -left_FL.x, left_FL.y,
        //    left_FR.z, -left_FR.x, left_FR.y,
        //    left_RR.z, -left_RR.x, left_RR.y,
        //    left_RL.z, -left_RL.x, left_RL.y
        //};

        sw.Stop(); // 타이머 중지
        //UnityEngine.Debug.Log($"receiveSimulationResult 실행 시간: {sw.Elapsed.TotalMilliseconds:F3} ms"); // 소수점 3자리까지 출력

        sw.Restart();
    }
    private bool hasCollided = false;
    private void OnTriggerEnter(Collider other)
    {
        if (hasCollided) return;
        // 충돌한 오브젝트의 레이어가 Map 레이어인지 확인
        if ((layer_mask & (1 << other.gameObject.layer)) != 0)
        {
            UnityEngine.Debug.Log("차가 건물(Map 레이어)에 부딪혔습니다! 충돌좌표 :"+ transform.position);
            //FMI.simulationResult.Clear();
            OnCarCollision?.Invoke();
            collision = true;
            hasCollided = true;
        }
    }
    public void CollisionResetCarPosition()
    {
        // 차량의 위치와 회전을 초기값으로 설정
        FMI.initialize_executed = false;
        initial_position = new Vector3(transform.position.x - transform.forward.x * 5, 0, transform.position.z - transform.forward.z * 5);
        value_y = 0; value_x = 0;
        collision = false;           
        hasCollided = false;
    }

    public void ResetCarPosition()
    {
        initial_position = new Vector3(0, 0, 0);
        FMI.initialize_executed = false;
    }
}
