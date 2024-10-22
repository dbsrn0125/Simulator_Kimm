using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    Light RedLight_L;
    Light YellowLight_L;
    Light GreenLight_L;
    Light GreenLight_R;
    Light[] Lights;

    float timer = 0.0f;

    void Awake()
    {
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

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < 10f)
        {
            RedLight_L.enabled = false;
            YellowLight_L.enabled = false;
            GreenLight_L.enabled = false;
            GreenLight_R.enabled = true;
        }

        else if (timer < 13f)
        {
            RedLight_L.enabled = false;
            YellowLight_L.enabled = true;
            GreenLight_L.enabled = false;
            GreenLight_R.enabled = false;
        }
        else if (timer < 23f)
        {
            RedLight_L.enabled = true;
            YellowLight_L.enabled = false;
            GreenLight_L.enabled = true;
            GreenLight_R.enabled = false;
        }
        else if (timer < 26f)
        {
            RedLight_L.enabled = false;
            YellowLight_L.enabled = true;
            GreenLight_L.enabled = false;
            GreenLight_R.enabled = false;
        }
        else if (timer < 36f)
        {
            RedLight_L.enabled = true;
            YellowLight_L.enabled = false;
            GreenLight_L.enabled = false;
            GreenLight_R.enabled = false;
        }

        else
        {
            timer = 0.0f;
        }
    }
}
