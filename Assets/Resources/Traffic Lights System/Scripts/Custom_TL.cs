using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_TL : MonoBehaviour
{
    public Renderer RedRenderer;
    public GameObject RedHalo;

    public Renderer YellowRenderer;
    public GameObject YellowHalo;

    public Renderer GreenRenderer;
    public GameObject GreenHalo;

    public Material LightsOnMat;
    public Material LightsOffMat;

    private bool mInitialized = false;

    public bool redLightState = true;

    public bool yellowLightState = true;

    public bool greenLightState = true;

    // Start is called before the first frame update
    void Start()
    {
        if (RedHalo != null)
            RedHalo.SetActive(redLightState);

        if (RedRenderer != null)
            RedRenderer.material = (redLightState) ? LightsOnMat : LightsOffMat;

        if (YellowHalo != null)
            YellowHalo.SetActive(yellowLightState);

        if (YellowRenderer != null)
            YellowRenderer.material = (yellowLightState) ? LightsOnMat : LightsOffMat;

        if (GreenHalo != null)
            GreenHalo.SetActive(greenLightState);

        if (GreenRenderer != null)
            GreenRenderer.material = (greenLightState) ? LightsOnMat : LightsOffMat;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
