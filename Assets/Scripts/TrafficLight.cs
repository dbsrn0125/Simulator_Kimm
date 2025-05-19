using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhaseEntry
{
    public TrafficLightStatus status;
    [Range(0f, 60f)] public float duration;
}

public class TrafficLight : MonoBehaviour
{
    // private static int nextID = 1;  // ID
    public int ID; // { get; private set; }
    
    [Header("Traffic Light Status Sequence")]
    public List<PhaseEntry> statusSequence = new List<PhaseEntry>();
    private int currentPhaseIndex = 0;

    public TrafficLightStatus status { get; private set; }

    Light RedLight;
    Light YellowLight;
    Light GreenLight;
    Light LeftLight;
    Light[] Lights;

    // float timer = 0.0f;

    public void StartCycle()
    {
        StopAllCoroutines();
        StartCoroutine(CycleCoroutine());
    }

    private IEnumerator CycleCoroutine()
    {
        while (true)
        {
            var phase = statusSequence[currentPhaseIndex];
            SetStatus(phase.status);
            
            yield return new WaitForSeconds(phase.duration);

            currentPhaseIndex = (currentPhaseIndex + 1) % statusSequence.Count;
        }
    }

    void Awake()
    {   
        // ID = nextID++; // 오브젝트 생성 시 ID 할당 후 증가
        status = TrafficLightStatus.Red;

        Lights = GetComponentsInChildren<Light>();
        foreach (Light i in Lights)
        {
            if (i.name == "red_light")
            {
                RedLight = i.GetComponent<Light>();
            }
            else if (i.name == "yellow_light")
            {
                YellowLight = i.GetComponent<Light>();
            }
            else if (i.name == "green_light")
            {
                GreenLight = i.GetComponent<Light>();
            }
            else if (i.name == "green_light_l")
            {
                LeftLight = i.GetComponent<Light>();
            }
        }
    }

    public void SetStatus(TrafficLightStatus _status)
    {
        status = _status;
        
        // 모든 조명을 끄기
        foreach (Light light in Lights)
        {
            if (light != null) light.enabled = false;
        }

        // 현재 상태에 맞는 조명 켜기
        switch (_status)
        {
            case TrafficLightStatus.Red:
                if (RedLight != null) RedLight.enabled = true;
                break;
            case TrafficLightStatus.Yellow:
                if (YellowLight != null) YellowLight.enabled = true;
                break;
            case TrafficLightStatus.Green:
                if (GreenLight != null) GreenLight.enabled = true;
                break;
            case TrafficLightStatus.Left:
                if (LeftLight != null) LeftLight.enabled = true;
                break;
            case TrafficLightStatus.LeftGreen:
                if (GreenLight != null) GreenLight.enabled = true;
                if (LeftLight != null) LeftLight.enabled = true;
                break;
            case TrafficLightStatus.LeftRed:
                if (GreenLight != null) RedLight.enabled = true;
                if (LeftLight != null) LeftLight.enabled = true;
                break;
        }
    }

    public TrafficLightStatus GetStatus()
    {
        return status;
    }

    // void Update()
    // {   
    // }
}
