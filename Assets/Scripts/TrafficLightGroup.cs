using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightGroup : MonoBehaviour
{
    [SerializeField] private List<TrafficLight> trafficLights = new List<TrafficLight>();

    public IReadOnlyList<TrafficLight> TrafficLightsReadonly => trafficLights;

    private TrafficLightStatus status;

    void OnValidate()
    {   
        trafficLights.Clear();
        foreach (Transform child in transform)
        {
            TrafficLight light = child.GetComponent<TrafficLight>();
            if (light != null)
            {
                trafficLights.Add(light);
            }
        }
    }

    void Start()
    {
        foreach (var light in trafficLights)
        {
            light.StartCycle();
        }
    }

    // void Update()
    // {
    
    // }

    public List<TrafficLight> GetTrafficLights()
    {
        return trafficLights;
    }
    
}
