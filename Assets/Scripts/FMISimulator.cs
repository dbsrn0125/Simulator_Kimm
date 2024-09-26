using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Scripting.Python;
using Python.Runtime;
using System;
using System.Diagnostics;

public class FMISimulator : MonoBehaviour
{
    dynamic FMI;
    public float realCurrentTime = 0f;
    InputDeviceController device;
    Dictionary<string, int> vrs;
    List<int> get_key = new List<int>();
    List<double> get_value;
    int vr_Vx;
    int vr_roll;
    int vr_pitch;
    int vr_yaw;

    int vr_Accel;
    int vr_brake;
    int vr_gear;
    int vr_steer;

    int vr_ray_FL;
    int vr_ray_FR;
    int vr_ray_RR;
    int vr_ray_RL;

    int vr_forward_x_FL;
    int vr_forward_y_FL;
    int vr_forward_z_FL;
    int vr_left_x_FL;
    int vr_left_y_FL;
    int vr_left_z_FL;

    int vr_forward_x_FR;
    int vr_forward_y_FR;
    int vr_forward_z_FR;
    int vr_left_x_FR;
    int vr_left_y_FR;
    int vr_left_z_FR;

    int vr_forward_x_RR;
    int vr_forward_y_RR;
    int vr_forward_z_RR;
    int vr_left_x_RR;
    int vr_left_y_RR;
    int vr_left_z_RR;

    int vr_forward_x_RL;
    int vr_forward_y_RL;
    int vr_forward_z_RL;
    int vr_left_x_RL;
    int vr_left_y_RL;
    int vr_left_z_RL;

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
    public List<double> simulationResult = new List<double>();

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
        sw.Start();
        PythonRunner.EnsureInitialized();
        using(Py.GIL())
        {
            //if (FMI)
            //{
            //    FMI.simulate_free();
            //}
            dynamic FMI_py = Py.Import("custom_input_test");
            FMI = FMI_py.FMI_manager(Environment.CurrentDirectory + "\\Assets\\" + "\\fmu\\" + "KIMM_CAR.fmu");
            vrs = FMI.get_vrs();          
        }
        #region variables

        var vr_x = vrs["body_Model.x"];
        var vr_y = vrs["body_Model.y"];
        var vr_z = vrs["body_Model.z"];

        var vr_Vx = vrs["body_Model.dx"];
        var vr_Vy = vrs["body_Model.dy"];
        var vr_Vz = vrs["body_Model.dz"];

        var vr_Vx_B = vrs["body_Model.body_fixed_dx"];
        var vr_Vy_B = vrs["body_Model.body_fixed_dy"];
        var vr_Vz_B = vrs["body_Model.body_fixed_dz"];

        var vr_tire_w1 = vrs["tire_front_left.roll"];
        var vr_tire_w2 = vrs["tire_front_right.roll"];
        var vr_tire_w3 = vrs["tire_rear_right.roll"];
        var vr_tire_w4 = vrs["tire_rear_left.roll"];

        var vr_rotation_radius = vrs["ackerman_Steering_Model.left_steer_angle"];

        var vr_unsprung_z1 = vrs["suspension_front_left.suspension_comp_dist"];
        var vr_unsprung_z2 = vrs["suspension_front_right.suspension_comp_dist"];
        var vr_unsprung_z3 = vrs["suspension_rear_right.suspension_comp_dist"];
        var vr_unsprung_z4 = vrs["suspension_rear_left.suspension_comp_dist"];

        var vr_left_steer = vrs["ackerman_Steering_Model.left_steer_angle"];
        var vr_right_steer = vrs["ackerman_Steering_Model.right_steer_angle"];
        var vr_w = vrs["body_Model.d_yaw"];
        //var vr_we = vrs["ev_motor_model.w"];
        var vr_we = vrs["ev_motor_model.shaft_rotation_speed"];
        var vr_nf = vrs["tire_front_right.tire_normal_force"];

        var vr_nl_fl = vrs["tire_front_left.d_roll"];
        //var vr_sa_fl = vrs["rear_open_differential.w_right"];
        var vr_sa_fl = vrs["tire_front_right.d_roll"];
        var vr_tr_fl = vrs["body_Model.dx"];
        var vr_s_fl = vrs["body_Model.dy"];

        var vr_nl_rl = vrs["tire_rear_left.d_roll"];
        var vr_sa_rl = vrs["tire_rear_right.d_roll"];
        var vr_tr_rl = vrs["tire_rear_left.slip_ratio"];
        var vr_s_rl = vrs["tire_rear_right.slip_ratio"];
        //var vr_sa_rl = vrs["tire_rear_left.body_vy"];
        //var vr_tr_rl = vrs["tire_rear_right.d_roll"];
        //var vr_s_rl = vrs["tire_rear_left.slip_angle"];

        //var vr_nl_fl = vrs["tire_front_left.d_roll"];
        //var vr_sa_fl = vrs["tire_front_right.d_roll"];
        //var vr_tr_fl = vrs["tire_rear_left.d_roll"];
        //var vr_s_fl = vrs["tire_rear_right.d_roll"];

        //var vr_nl_rl = vrs["body_Model.roll"];
        //var vr_sa_rl = vrs["body_Model.pitch"];
        //var vr_tr_rl = vrs["body_Model.yaw"];
        //var vr_s_rl = vrs["body_Model.roll"];

        vr_roll = vrs["body_Model.roll"];
        vr_pitch = vrs["body_Model.pitch"];
        vr_yaw = vrs["body_Model.yaw"];

        vr_Accel = vrs["acceleration.k"];
        vr_steer = vrs["angle_steer.k"];
        vr_brake = vrs["brake.k"];
        vr_gear = vrs["transmission.k"];

        vr_ray_FL = vrs["ray_front_left.k"];
        vr_ray_FR = vrs["ray_front_right.k"];
        vr_ray_RR = vrs["ray_rear_right.k"];
        vr_ray_RL = vrs["ray_rear_left.k"];

        vr_forward_x_FL = vrs["front_x_fl.k"];
        vr_forward_y_FL = vrs["front_y_fl.k"];
        vr_forward_z_FL = vrs["front_z_fl.k"];
        vr_left_x_FL = vrs["left_x_fl.k"];
        vr_left_y_FL = vrs["left_y_fl.k"];
        vr_left_z_FL = vrs["left_z_fl.k"];

        vr_forward_x_FR = vrs["front_x_fr.k"];
        vr_forward_y_FR = vrs["front_y_fr.k"];
        vr_forward_z_FR = vrs["front_z_fr.k"];
        vr_left_x_FR = vrs["left_x_fr.k"];
        vr_left_y_FR = vrs["left_y_fr.k"];
        vr_left_z_FR = vrs["left_z_fr.k"];

        vr_forward_x_RR = vrs["front_x_rr.k"];
        vr_forward_y_RR = vrs["front_y_rr.k"];
        vr_forward_z_RR = vrs["front_z_rr.k"];
        vr_left_x_RR = vrs["left_x_rr.k"];
        vr_left_y_RR = vrs["left_y_rr.k"];
        vr_left_z_RR = vrs["left_z_rr.k"];

        vr_forward_x_RL = vrs["front_x_rl.k"];
        vr_forward_y_RL = vrs["front_y_rl.k"];
        vr_forward_z_RL = vrs["front_z_rl.k"];
        vr_left_x_RL = vrs["left_x_rl.k"];
        vr_left_y_RL = vrs["left_y_rl.k"];
        vr_left_z_RL = vrs["left_z_rl.k"];

        get_key.Add(vr_x);      // 0
        get_key.Add(vr_y);      // 1
        get_key.Add(vr_yaw);     // 2
        get_key.Add(vr_left_steer);       // 3
        get_key.Add(vr_right_steer);      // 4
        get_key.Add(vr_z);      // 5
        get_key.Add(vr_roll);    // 6
        get_key.Add(vr_pitch);   // 7
        get_key.Add(vr_unsprung_z1); // 8
        get_key.Add(vr_unsprung_z2); // 9
        get_key.Add(vr_unsprung_z3); // 10
        get_key.Add(vr_unsprung_z4); // 11
        get_key.Add(vr_Vx);          // 12  
        get_key.Add(vr_Vy);          // 13
        get_key.Add(vr_tire_w1);     // 14
        get_key.Add(vr_tire_w2);     // 15
        get_key.Add(vr_tire_w3);     // 16
        get_key.Add(vr_tire_w4);     // 17
        get_key.Add(vr_we);// 18
        get_key.Add(vr_Vx_B);// 19
        get_key.Add(vr_Vy_B);// 20
        get_key.Add(vr_nl_fl);// 21
        get_key.Add(vr_sa_fl);// 22
        get_key.Add(vr_tr_fl);// 23
        get_key.Add(vr_s_fl);// 24
        get_key.Add(vr_nl_rl);// 25
        get_key.Add(vr_sa_rl);// 26
        get_key.Add(vr_tr_rl);// 27
        get_key.Add(vr_s_rl);// 28

        #endregion

        Simulation_Initial_Set();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //UnityEngine.Debug.Log(Time.deltaTime);
        realCurrentTime += Time.deltaTime;
        if(initialize_executed)
        {
            Simulation_Do_Step();
            //while ((sw.Elapsed - loop_start).TotalMilliseconds < 0.001) { }
        }
        
    }
    private void Simulation_Initial_Set()
    {
        Dictionary<int, double> Dic_set = new Dictionary<int, double>();
        List<int> set_key = new List<int>(Dic_set.Keys);
        List<double> set_value = new List<double>(Dic_set.Values);
        using (Py.GIL())
        {
            FMI.simulate_init_set(set_key, set_value);
        }
    }
    
    private void Simulation_Do_Step()
    {
        //UnityEngine.Debug.Log($"throttle:{device.throttleInput}, brake:{device.brakeInput}, gear:{device.gearInput}, steer: {device.steeringInput}");
        Dictionary<int, double> Dic_set = new Dictionary<int, double>();
        Dic_set.Add(vr_Accel, device.throttleInput);
        Dic_set.Add(vr_brake, device.brakeInput);
        Dic_set.Add(vr_gear, device.gearInput);
        Dic_set.Add(vr_steer, -device.steeringInput);
        Dic_set.Add(vr_ray_FL, ray_FL);
        Dic_set.Add(vr_ray_FR, ray_FR);
        Dic_set.Add(vr_ray_RR, ray_RR);
        Dic_set.Add(vr_ray_RL, ray_RL);

        Dic_set.Add(vr_forward_x_FL, forward_FL[0]);
        Dic_set.Add(vr_forward_y_FL, forward_FL[1]);
        Dic_set.Add(vr_forward_z_FL, forward_FL[2]);
        Dic_set.Add(vr_left_x_FL, left_FL[0]);
        Dic_set.Add(vr_left_y_FL, left_FL[1]);
        Dic_set.Add(vr_left_z_FL, left_FL[2]);

        Dic_set.Add(vr_forward_x_FR, forward_FR[0]);
        Dic_set.Add(vr_forward_y_FR, forward_FR[1]);
        Dic_set.Add(vr_forward_z_FR, forward_FR[2]);
        Dic_set.Add(vr_left_x_FR, left_FR[0]);
        Dic_set.Add(vr_left_y_FR, left_FR[1]);
        Dic_set.Add(vr_left_z_FR, left_FR[2]);

        Dic_set.Add(vr_forward_x_RR, forward_RR[0]);
        Dic_set.Add(vr_forward_y_RR, forward_RR[1]);
        Dic_set.Add(vr_forward_z_RR, forward_RR[2]);
        Dic_set.Add(vr_left_x_RR, left_RR[0]);
        Dic_set.Add(vr_left_y_RR, left_RR[1]);
        Dic_set.Add(vr_left_z_RR, left_RR[2]);

        Dic_set.Add(vr_forward_x_RL, forward_RL[0]);
        Dic_set.Add(vr_forward_y_RL, forward_RL[1]);
        Dic_set.Add(vr_forward_z_RL, forward_RL[2]);
        Dic_set.Add(vr_left_x_RL, left_RL[0]);
        Dic_set.Add(vr_left_y_RL, left_RL[1]);
        Dic_set.Add(vr_left_z_RL, left_RL[2]);

        List<int> set_key = new List<int>(Dic_set.Keys);
        List<double> set_value = new List<double>(Dic_set.Values);

        using (Py.GIL())
        {
            // if prev_time is None:
            //    prev_time = clock
            //    return 
            // cur_time => clock 라이브러리
            // time_interval = cur_time - prev_time
            get_value = FMI.simulate_step(0.005, set_key, set_value, get_key);
            // prev_time = cur_time
        }
        simulationResult.Add((double)FMI.current_time);
        simulationResult.Add(get_value[0]);
        simulationResult.Add(get_value[1]);
        simulationResult.Add(get_value[2] * 180.0f / Mathf.PI);
        simulationResult.Add(get_value[5]);
        simulationResult.Add(get_value[6] * 180.0f / Mathf.PI);
        simulationResult.Add(get_value[7] * 180.0f / Mathf.PI);
        simulationResult.Add(get_value[8]);
        simulationResult.Add(get_value[9]);
        simulationResult.Add(get_value[10]);
        simulationResult.Add(get_value[11]);
        simulationResult.Add(get_value[3] * 180.0f / Mathf.PI);
        simulationResult.Add(get_value[4] * 180.0f / Mathf.PI);
        simulationResult.Add(get_value[12]);
        simulationResult.Add(get_value[13]);
        simulationResult.Add(get_value[14] * 180.0f / Mathf.PI);
        simulationResult.Add(get_value[15] * 180.0f / Mathf.PI);
        simulationResult.Add(get_value[16] * 180.0f / Mathf.PI);
        simulationResult.Add(get_value[17] * 180.0f / Mathf.PI);
        simulationResult.Add(get_value[19]);
        simulationResult.Add(get_value[20]);
        transform.GetComponent<CarController>().receiveSimulationResult(simulationResult);
        simulationResult.Clear();
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
    private void OnApplicationQuit()
    {
        using(Py.GIL())
        {
            FMI.simulate_free();
            Simulation_Initial_Set();
        }
    }
}
