using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Scripting.Python;
using Python.Runtime;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

public class CarSimFMI : MonoBehaviour
{
    dynamic carsim_FMI;

    Dictionary<string, int> vrs_input;
    int[] vrs_input_indices;

    Dictionary<string, int> vrs_output;
    int[] vrs_output_indices;
    float[] simulation_results;
    
    // simulation results
    public Vector3 position;

    void Start()
    {
        string fmu_path = Environment.CurrentDirectory + "\\Assets\\" + "\\Resources\\" + "\\fmu\\" + "TruckSim_0718_NotSelfCon.fmu";
        float start_time = 0.0f;
        float step_size = 0.001f;

        PythonRunner.EnsureInitialized();
        using (Py.GIL())
        {
            dynamic FMI_py = Py.Import("carsim_fmi");
            carsim_FMI = FMI_py.CarSimFMI(fmu_path, start_time, step_size);

            var vrs_input_ = carsim_FMI.get_vrs_input();
            var vrs_output_ = carsim_FMI.get_vrs_output();
            vrs_input = vrs_input_.As<Dictionary<string, int>>();
            vrs_output = vrs_output_.As<Dictionary<string, int>>();

            if (!carsim_FMI.initialized)
            {
                UnityEngine.Debug.LogError("CarSim initialization failed.");
                UnityEditor.EditorApplication.isPlaying = false; // stop simulating
                return;
            }
        }

        UnityEngine.Debug.Log("CarSim Start Successfully");

        #region variables
        vrs_output_indices = new int[] { vrs_output["Xo"], vrs_output["Yo"], vrs_output["Zo"] };
        simulation_results = new float[vrs_output_indices.Length];
        #endregion
    }

    void FixedUpdate()
    {
        if (carsim_FMI == null)
        {
            UnityEngine.Debug.LogError("CarSim FMU is not initialized.");
            return;
        }

        Simulate();
        UpdatePosition();
    }
    
    void Simulate()
    {
        try
        {
            using (Py.GIL())
            {
                var output_ = carsim_FMI.simulate(vrs_output_indices);
                UnityEngine.Debug.Log("CarSim Step Successfully");
                
                for (int i = 0; i < simulation_results.Length; i++)
                {
                    simulation_results[i] = output_[i].As<float>();
                }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error during simulation step: {ex.Message}");
        }
    }

    void UpdatePosition()
    {
        position.x = simulation_results[1];
        position.y = simulation_results[2];
        position.z = simulation_results[0];
    }

    private async void OnDisable()
    {
        try
        {
            UnityEngine.Debug.Log("Starting FMU cleanup...");

            if (carsim_FMI != null)
            {
                using (Py.GIL())
                {
                    carsim_FMI.terminate();  // 반드시 명시적 종료
                    carsim_FMI = null;
                }
            }

            UnityEngine.Debug.Log("FMU cleanup completed.");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error during FMU cleanup: {ex.Message}");
        }
    }
}