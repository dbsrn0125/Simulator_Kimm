using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Scripting.Python;
using Python.Runtime;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

public class FMISimulator : MonoBehaviour
{
    dynamic FMI;
    public float realCurrentTime = 0f;
    InputDeviceController device;
    List<int> set_key;
    List<float> set_value;
    List<int> get_key = new List<int>();
    List<double> get_value; // Python: Float => C#: double / 64비트
    List<float> simulationResult;

    float ray_FL = 1.0f, ray_FR = 1.0f, ray_RL = 1.0f, ray_RR = 1.0f;

    float[] forward_FL = new float[3] { 1.0f, 0.0f, 0.0f };
    float[] forward_FR = new float[3] { 1.0f, 0.0f, 0.0f };
    float[] forward_RR = new float[3] { 1.0f, 0.0f, 0.0f };
    float[] forward_RL = new float[3] { 1.0f, 0.0f, 0.0f };

    float[] left_FL = new float[3] { 0.0f, 1.0f, 0.0f };
    float[] left_FR = new float[3] { 0.0f, 1.0f, 0.0f };
    float[] left_RR = new float[3] { 0.0f, 1.0f, 0.0f };
    float[] left_RL = new float[3] { 0.0f, 1.0f, 0.0f };

    bool initialize_executed = true;
   
    TimeSpan loop_start;
    Stopwatch sw;
    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(Environment.CurrentDirectory);
        device = transform.GetComponent<InputDeviceController>();
        sw = new Stopwatch();
        
        Dictionary<string, int> vrs;

        PythonRunner.EnsureInitialized();
        using (Py.GIL())
        {
            dynamic FMI_py = Py.Import("custom_input_test");
            FMI = FMI_py.FMI_manager(Environment.CurrentDirectory + "\\Assets\\" + "\\fmu\\" + "KIMM_CAR.fmu");
            FMI.simulate_init();
            vrs = FMI.get_vrs();
        }
        #region variables

        int vr_x = vrs["body_Model.x"];
        int vr_y = vrs["body_Model.y"];
        int vr_z = vrs["body_Model.z"];

        int vr_Vx = vrs["body_Model.dx"];
        int vr_Vy = vrs["body_Model.dy"];
        int vr_Vz = vrs["body_Model.dz"];

        int vr_Vx_B = vrs["body_Model.body_fixed_dx"];
        int vr_Vy_B = vrs["body_Model.body_fixed_dy"];
        int vr_Vz_B = vrs["body_Model.body_fixed_dz"];

        int vr_tire_w1 = vrs["tire_front_left.roll"];
        int vr_tire_w2 = vrs["tire_front_right.roll"];
        int vr_tire_w3 = vrs["tire_rear_right.roll"];
        int vr_tire_w4 = vrs["tire_rear_left.roll"];

        int vr_rotation_radius = vrs["ackerman_Steering_Model.left_steer_angle"];

        int vr_unsprung_z1 = vrs["suspension_front_left.suspension_comp_dist"];
        int vr_unsprung_z2 = vrs["suspension_front_right.suspension_comp_dist"];
        int vr_unsprung_z3 = vrs["suspension_rear_right.suspension_comp_dist"];
        int vr_unsprung_z4 = vrs["suspension_rear_left.suspension_comp_dist"];

        int vr_left_steer = vrs["ackerman_Steering_Model.left_steer_angle"];
        int vr_right_steer = vrs["ackerman_Steering_Model.right_steer_angle"];
        int vr_w = vrs["body_Model.d_yaw"];
        int vr_we = vrs["ev_motor_model.shaft_rotation_speed"];
        int vr_nf = vrs["tire_front_right.tire_normal_force"];

        int vr_nl_fl = vrs["tire_front_left.d_roll"];
        int vr_sa_fl = vrs["tire_front_right.d_roll"];
        int vr_tr_fl = vrs["body_Model.dx"];
        int vr_s_fl = vrs["body_Model.dy"];

        int vr_nl_rl = vrs["tire_rear_left.d_roll"];
        int vr_sa_rl = vrs["tire_rear_right.d_roll"];
        int vr_tr_rl = vrs["tire_rear_left.slip_ratio"];
        int vr_s_rl = vrs["tire_rear_right.slip_ratio"];

        int vr_roll = vrs["body_Model.roll"];
        int vr_pitch = vrs["body_Model.pitch"];
        int vr_yaw = vrs["body_Model.yaw"];

        int vr_Accel = vrs["acceleration.k"];
        int vr_steer = vrs["angle_steer.k"];
        int vr_brake = vrs["brake.k"];
        int vr_gear = vrs["transmission.k"];

        int vr_ray_FL = vrs["ray_front_left.k"];
        int vr_ray_FR = vrs["ray_front_right.k"];
        int vr_ray_RR = vrs["ray_rear_right.k"];
        int vr_ray_RL = vrs["ray_rear_left.k"];

        int vr_forward_x_FL = vrs["front_x_fl.k"];
        int vr_forward_y_FL = vrs["front_y_fl.k"];
        int vr_forward_z_FL = vrs["front_z_fl.k"];
        int vr_left_x_FL = vrs["left_x_fl.k"];
        int vr_left_y_FL = vrs["left_y_fl.k"];
        int vr_left_z_FL = vrs["left_z_fl.k"];

        int vr_forward_x_FR = vrs["front_x_fr.k"];
        int vr_forward_y_FR = vrs["front_y_fr.k"];
        int vr_forward_z_FR = vrs["front_z_fr.k"];
        int vr_left_x_FR = vrs["left_x_fr.k"];
        int vr_left_y_FR = vrs["left_y_fr.k"];
        int vr_left_z_FR = vrs["left_z_fr.k"];

        int vr_forward_x_RR = vrs["front_x_rr.k"];
        int vr_forward_y_RR = vrs["front_y_rr.k"];
        int vr_forward_z_RR = vrs["front_z_rr.k"];
        int vr_left_x_RR = vrs["left_x_rr.k"];
        int vr_left_y_RR = vrs["left_y_rr.k"];
        int vr_left_z_RR = vrs["left_z_rr.k"];

        int vr_forward_x_RL = vrs["front_x_rl.k"];
        int vr_forward_y_RL = vrs["front_y_rl.k"];
        int vr_forward_z_RL = vrs["front_z_rl.k"];
        int vr_left_x_RL = vrs["left_x_rl.k"];
        int vr_left_y_RL = vrs["left_y_rl.k"];
        int vr_left_z_RL = vrs["left_z_rl.k"];

        set_key = new List<int>
        {
            vr_Accel, vr_brake, vr_gear, vr_steer, vr_ray_FL, vr_ray_FR, vr_ray_RR, vr_ray_RL,
            vr_forward_x_FL, vr_forward_y_FL, vr_forward_z_FL, vr_left_x_FL, vr_left_y_FL, vr_left_z_FL,
            vr_forward_x_FR, vr_forward_y_FR, vr_forward_z_FR, vr_left_x_FR, vr_left_y_FR, vr_left_z_FR,
            vr_forward_x_RR, vr_forward_y_RR, vr_forward_z_RR, vr_left_x_RR, vr_left_y_RR, vr_left_z_RR,
            vr_forward_x_RL, vr_forward_y_RL, vr_forward_z_RL, vr_left_x_RL, vr_left_y_RL, vr_left_z_RL
        };

        UnityEngine.Debug.Log($"vr_steer: {vr_steer}, vr_brake: {vr_brake}");

        get_key = new List<int>
        {
            vr_x,           // 0
            vr_y,           // 1
            vr_yaw,         // 2
            vr_left_steer,  // 3
            vr_right_steer, // 4
            vr_z,           // 5
            vr_roll,        // 6
            vr_pitch,       // 7
            vr_unsprung_z1, // 8
            vr_unsprung_z2, // 9
            vr_unsprung_z3, // 10
            vr_unsprung_z4, // 11
            vr_Vx,          // 12
            vr_Vy,          // 13
            vr_tire_w1,     // 14
            vr_tire_w2,     // 15
            vr_tire_w3,     // 16
            vr_tire_w4,     // 17
            vr_we,          // 18
            vr_Vx_B,        // 19
            vr_Vy_B,        // 20
            vr_nl_fl,       // 21
            vr_sa_fl,       // 22
            vr_tr_fl,       // 23
            vr_s_fl,        // 24
            vr_nl_rl,       // 25
            vr_sa_rl,       // 26
            vr_tr_rl,       // 27
            vr_s_rl         // 28
        };
        #endregion
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        sw.Restart();
        realCurrentTime += Time.deltaTime;
        if (initialize_executed)
        {
            Simulation_Do_Step();
        }

        sw.Stop(); // 타이머 중지
        UnityEngine.Debug.Log($"FMI Simulator 처리 시간: {sw.Elapsed.TotalMilliseconds:F3} ms"); // 소수점 3자리까지 출력
    }

    void Update()
    {
    }

    private void Simulation_Do_Step()
    {
        set_value = new List<float>
        {
            device.throttleInput, device.brakeInput, device.gearInput, -device.steeringInput,
            ray_FL, ray_FR, ray_RR, ray_RL,
            forward_FL[0], forward_FL[1], forward_FL[2], left_FL[0], left_FL[1], left_FL[2],
            forward_FR[0], forward_FR[1], forward_FR[2], left_FR[0], left_FR[1], left_FR[2],
            forward_RR[0], forward_RR[1], forward_RR[2], left_RR[0], left_RR[1], left_RR[2],
            forward_RL[0], forward_RL[1], forward_RL[2], left_RL[0], left_RL[1], left_RL[2]
        };
        try
        {
            using (Py.GIL())
            {
                var pythonSetKey = new PyList(set_key.Select(k => new PyInt(k)).ToArray());
                var pythonSetValue = new PyList(set_value.Select(v => new PyFloat(v)).ToArray());
                get_value = FMI.simulate_step(0.001, pythonSetKey, pythonSetValue, get_key);
            }
        }
        catch(Exception ex)
        {
            UnityEngine.Debug.LogError($"Error during simulation step: {ex.Message}");
        }
        simulationResult = new List<float>
        {
            (float) FMI.current_time,
            (float) get_value[0],
            (float) get_value[1],
            (float) get_value[2] * 180.0f / Mathf.PI,
            (float) get_value[5],
            (float) get_value[6] * 180.0f / Mathf.PI,
            (float) get_value[7] * 180.0f / Mathf.PI,
            (float) get_value[8],
            (float) get_value[9],
            (float) get_value[10],
            (float) get_value[11],
            (float) get_value[3] * 180.0f / Mathf.PI,
            (float) get_value[4] * 180.0f / Mathf.PI,
            (float) get_value[12],
            (float) get_value[13],
            (float) get_value[14] * 180.0f / Mathf.PI,
            (float) get_value[15] * 180.0f / Mathf.PI,
            (float) get_value[16] * 180.0f / Mathf.PI,
            (float) get_value[17] * 180.0f / Mathf.PI,
            (float) get_value[19],
            (float) get_value[20]
        };

        transform.GetComponent<CarController>().receiveSimulationResult(simulationResult);
    }

    public void receiveRayInfo(List<float> rayInfo)
    {
        //Debug.Log("Receiveing Ray Info");
        ray_FL = rayInfo[0];
        ray_FR = rayInfo[1];
        ray_RR = rayInfo[2];
        ray_RL = rayInfo[3];

        forward_FL[0] = rayInfo[4];
        forward_FL[1] = rayInfo[5];
        forward_FL[2] = rayInfo[6];

        forward_FR[0] = rayInfo[7];
        forward_FR[1] = rayInfo[8];
        forward_FR[2] = rayInfo[9];

        forward_RR[0] = rayInfo[10];
        forward_RR[1] = rayInfo[11];
        forward_RR[2] = rayInfo[12];

        forward_RL[0] = rayInfo[13];
        forward_RL[1] = rayInfo[14];
        forward_RL[2] = rayInfo[15];

        left_FL[0] = rayInfo[16];
        left_FL[1] = rayInfo[17];
        left_FL[2] = rayInfo[18];

        left_FR[0] = rayInfo[19];
        left_FR[1] = rayInfo[20];
        left_FR[2] = rayInfo[21];

        left_RR[0] = rayInfo[22];
        left_RR[1] = rayInfo[23];
        left_RR[2] = rayInfo[24];

        left_RL[0] = rayInfo[25];
        left_RL[1] = rayInfo[26];
        left_RL[2] = rayInfo[27];
        initialize_executed = true;
    }

    private async void OnDisable()
    {
        try
        {
            UnityEngine.Debug.Log("Starting FMU cleanup...");

            await Task.Run(() => {
                using (Py.GIL())
                {
                    if (FMI != null)
                    {
                        FMI.simulate_free();
                        FMI = null;
                    }
                }
            });

            UnityEngine.Debug.Log(FMI == null ? "FMU instance is null" : "FMU instance is not null");
            UnityEngine.Debug.Log("FMU cleanup completed.");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error during FMU cleanup: {ex.Message}");
        }
    }
}
