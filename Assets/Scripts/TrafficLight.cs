using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    private static int nextID = 1;  // ID
    public int id { get; private set; }
    
    public TrafficLightStatus status { get; private set; }

    Light RedLight_L;
    Light YellowLight_L;
    Light GreenLight_L;
    Light GreenLight_R;
    Light[] Lights;

    // float timer = 0.0f;

    void Awake()
    {   
        id = nextID++; // 오브젝트 생성 시 ID 할당 후 증가
        status = TrafficLightStatus.Red;

        Lights = GetComponentsInChildren<Light>();
        foreach (Light i in Lights)
        {
            if (i.name == "red_light")
            {
                RedLight_L = i.GetComponent<Light>();
            }
            else if (i.name == "yellow_light")
            {
                YellowLight_L = i.GetComponent<Light>();
            }
            else if (i.name == "green_light")
            {
                GreenLight_L = i.GetComponent<Light>();
            }
            else if (i.name == "green_light_l")
            {
                GreenLight_R = i.GetComponent<Light>();
            }
        }
    }

    public void SetStatus(TrafficLightStatus _status)
    {
        status = _status;
    }

    public TrafficLightStatus GetStatus()
    {
        return status;
    }

    // void statusUpdateSelf()
    // {
    //     timer += Time.deltaTime;

    //     if (timer < 10f)
    //     {
    //         status = TrafficLightStatus.Red;
    //     }

    //     else if (timer < 13f)
    //     {
    //         status = TrafficLightStatus.Yellow;
    //     }

    //     else if (timer < 23f)
    //     {
    //        status = TrafficLightStatus.Green;
    //     }
    //     else if (timer < 26f)
    //     {
    //         status = TrafficLightStatus.Yellow;
    //     }
    //     else if (timer < 36f)
    //     {
    //         status = TrafficLightStatus.LeftGreen;
    //     }
    //     else
    //     {
    //         timer = 0.0f;
    //     }
    // }

    void Update()
    {   
        // statusUpdateSelf();

        // 모든 조명을 끄기
        foreach (Light light in Lights)
        {
            if (light != null) light.enabled = false;
        }

        // 현재 상태에 맞는 조명 켜기
        switch (status)
        {
            case TrafficLightStatus.Red:
                if (RedLight_L != null) RedLight_L.enabled = true;
                break;
            case TrafficLightStatus.Yellow:
                if (YellowLight_L != null) YellowLight_L.enabled = true;
                break;
            case TrafficLightStatus.Green:
                if (GreenLight_L != null) GreenLight_L.enabled = true;
                break;
            case TrafficLightStatus.Left:
                if (GreenLight_R != null) GreenLight_R.enabled = true;
                break;
            case TrafficLightStatus.LeftGreen:
                if (GreenLight_L != null) GreenLight_L.enabled = true;
                if (GreenLight_R != null) GreenLight_R.enabled = true;
                break;
        }
    }
}
