using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket("ws://localhost:8080");
        ws.Connect();

        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        if(ws.IsAlive)
        {
            ws.Send(value.ToString());
        }
    }

    private void OnDestroy()
    {
        if(ws !=null)
        {
            ws.Close();
        }
    }
}
