using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class xController : MonoBehaviour
{
    public Transform ball;
    public Slider slider;
    public WebSocketManager webSocketManager;
    public PhysicMaterial pm;
    // Start is called before the first frame update
    void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 1;
        webSocketManager = FindObjectOfType<WebSocketManager>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        pm = ball.GetComponent<SphereCollider>().material;
    }

    void OnSliderValueChanged(float value)
    {
        pm.bounciness = value;
        if(webSocketManager != null)
        {
            webSocketManager.Send_Message($"SliderValue:{value}");
        }
    }
}
