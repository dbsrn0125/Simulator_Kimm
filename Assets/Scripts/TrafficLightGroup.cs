using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightGroup : MonoBehaviour
{
    private List<TrafficLight> trafficLights = new List<TrafficLight>();
    private TrafficLightStatus status;
    float timer = 0.0f;

    void Start()
    {
        foreach (Transform child in transform)
        {
            TrafficLight light = child.GetComponent<TrafficLight>();
            if (light != null)
            {
                trafficLights.Add(light);
            }
        }
    }

    
    void Update()
    {
        StatusUpdate();

        foreach (TrafficLight light in trafficLights)
        {
            light.SetStatus(status);
            // UnityEngine.Debug.Log(status);
        }

    }

    void StatusUpdate()
    {
        timer += Time.deltaTime;

        if (timer < 10f)
        {
            status = TrafficLightStatus.Red;
        }

        else if (timer < 13f)
        {
            status = TrafficLightStatus.Yellow;
        }

        else if (timer < 23f)
        {
           status = TrafficLightStatus.Green;
        }
        else if (timer < 26f)
        {
            status = TrafficLightStatus.Yellow;
        }
        else if (timer < 36f)
        {
            status = TrafficLightStatus.LeftGreen;
        }
        else
        {
            timer = 0.0f;
        }
    }

    public List<TrafficLight> GetTrafficLights()
    {
        return trafficLights;
    }
    
}
