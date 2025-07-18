using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Scripting.Python;
using Python.Runtime;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

public class CarSim : MonoBehaviour
{
    dynamic carsim_FMI;

    Dictionary<string, int> vrs_input;
    int[] vrs_input_indices;
    Dictionary<string, int> vrs_output;
    int[] vrs_output_indices;
    List<double> output;
    void Start()
    {
        string fmu_path = Environment.CurrentDirectory + "\\Assets\\" + "\\Resources\\" + "\\fmu\\" + "TruckSim_0717.fmu";
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

        // vrs_input_indices = new int[] { vrs_input["IMP_THROTTLE_ENGINE"],
        //                                 vrs_input["IMP_STEER_SW"],
        //                                 vrs_input["IMP_PCON_BK"],
        //                                 vrs_input["IMP_GEAR"]};
        // IMP_DZDX_L1I (Real): Input variable
        // IMP_DZDX_L1O (Real): Input variable
        // IMP_DZDX_L2I (Real): Input variable
        // IMP_DZDX_L2O (Real): Input variable
        // IMP_DZDX_R1I (Real): Input variable
        // IMP_DZDX_R1O (Real): Input variable
        // IMP_DZDX_R2I (Real): Input variable
        // IMP_DZDX_R2O (Real): Input variable
        // IMP_DZDY_L1I (Real): Input variable
        // IMP_DZDY_L1O (Real): Input variable
        // IMP_DZDY_L2I (Real): Input variable
        // IMP_DZDY_L2O (Real): Input variable
        // IMP_DZDY_R1I (Real): Input variable
        // IMP_DZDY_R1O (Real): Input variable
        // IMP_DZDY_R2I (Real): Input variable
        // IMP_DZDY_R2O (Real): Input variable
        vrs_output_indices = new int[] { vrs_output["Xo"], vrs_output["Yo"], vrs_output["Zo"] };
        
        #endregion
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        try
        {
            using (Py.GIL())
            {
                var output_ = carsim_FMI.simulate(vrs_output_indices);
                UnityEngine.Debug.Log("CarSim Step Successfully");
                UnityEngine.Debug.Log(output_);
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error during simulation step: {ex.Message}");
        }
    }
    
    private async void OnDisable()
    {
        try
        {
            UnityEngine.Debug.Log("Starting FMU cleanup...");

            await Task.Run(() => {
                using (Py.GIL())
                {
                    if (carsim_FMI == null)
                    {
                        return;
                    }
                    carsim_FMI.terminate();
                    carsim_FMI = null;
                }
            });

            UnityEngine.Debug.Log(carsim_FMI == null ? "FMU instance is null" : "FMU instance is not null");
            UnityEngine.Debug.Log("FMU cleanup completed.");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error during FMU cleanup: {ex.Message}");
        }
    }
}