using UnityEngine;
using WebSocketSharp;

public class WebSocketManager : MonoBehaviour
{
    private WebSocket ws;

    void Start()
    {
        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message from server: " + e.Data);
        };
        ws.Connect();
    }

    void OnDestroy()
    {
        ws.Close();
    }

    public void Send_Message(string message)
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Send(message);
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using WebSocketSharp;
//using UnityEngine.UI;

//public class WebSocketClient : MonoBehaviour
//{
//    private WebSocket ws;
//    public Slider slider;
//    // Start is called before the first frame update
//    void Start()
//    {
//        slider.onValueChanged.AddListener(OnSliderValueChanged);
//        ws = new WebSocket("ws://localhost:8080");
//        ws.Connect();
//    }

//    void OnSliderValueChanged(float value)
//    {
//        if (ws.IsAlive)
//        {
//            ws.Send(value.ToString());
//        }
//    }

//    private void OnDestroy()
//    {
//        if (ws != null)
//        {
//            ws.Close();
//        }
//    }
//}

