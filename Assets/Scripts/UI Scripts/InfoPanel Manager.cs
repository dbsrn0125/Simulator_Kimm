using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoPanelManager : MonoBehaviour
{
    public InputDeviceController IDC;
    public TextMeshProUGUI modeText;
    public TextMeshProUGUI gearText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(IDC.is_auto)
        {
            modeText.text = "Mode : Auto ";

        }
        else
        {
            modeText.text = "Mode : Manual";
        }
        gearText.text = "Gear: " + IDC.currentGear;
    }
}
